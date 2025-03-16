using System;
using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.LPostTube
{
    [Serializable]
    public class PostTubeObject
    {
        public string typeObject;
        public GameObject prefabObject;
    }

    public class PostTube : MonoBehaviour
    {
        [SerializeField] private PostBox postBox;
        
        [SerializeField] private Transform startPointObject;
        [SerializeField] private Transform downPointObject;
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private float timeFall = 1f;

        [SerializeField] private GameObject packageIfTriggerEnter;

        [SerializeField] private List<PostTubeObject> postTubeObjects;
        public UnityEvent<PostTubeObject> OnGetPostTubeObject;

        public void ObjectFall(GameObject prefab)
        {
            
            GameObject currentFallObject = Instantiate(prefab, startPointObject.position, prefab.transform.rotation);

            Collider colliderCurrentFallObject;
            Vector3 lastDownPointPosition = downPointObject.position;
            if (currentFallObject.TryGetComponent(out colliderCurrentFallObject))
            {
                downPointObject.position = new Vector3(downPointObject.position.x, downPointObject.position.y + colliderCurrentFallObject.bounds.size.y / 2, downPointObject.position.z);
            }

            MovePointToPoint movePointToPoint;
            if (currentFallObject.TryGetComponent(out movePointToPoint))
            {
                movePointToPoint.point1 = startPointObject;
                movePointToPoint.point2 = downPointObject;
                movePointToPoint.StartMoveTo(timeFall);

                StartCoroutine(ParticleFall(timeFall));
                StartCoroutine(TimeReturnDownPointPos(timeFall, lastDownPointPosition));
            }
            else
            {
                Debug.LogError("�� ������� � ����� ��� ������� ��������, �������� MovePointToPoint");
                downPointObject.position = lastDownPointPosition;
            }
            
        }

        public void ObjectFall(string typeObject)
        {
            for (int i = 0; i < postTubeObjects.Count; i++)
            {
                if (postTubeObjects[i].typeObject == typeObject)
                {
                    ObjectFall(postTubeObjects[i].prefabObject);
                    OnGetPostTubeObject?.Invoke(postTubeObjects[i]);

                    return;
                }
            }
        }

        public void ObjectFall(Reward reward)
        {
            ObjectFall(reward.typeReward);
        }

        IEnumerator ParticleFall(float t)
        {
            yield return new WaitForSeconds(t);
            particleSystem.Play();
            yield return new WaitForSeconds(0.2f);
            particleSystem.Stop();
        }
        IEnumerator TimeReturnDownPointPos(float t, Vector3 lastDownPointPosition)
        {
            yield return new WaitForSeconds(t);
            downPointObject.position = lastDownPointPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && packageIfTriggerEnter != null)
            {
                ObjectFall(packageIfTriggerEnter);
                packageIfTriggerEnter = null;
            }
        }
    }
}
