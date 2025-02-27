using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment.LPostTube
{
    public class PostTube : MonoBehaviour
    {
        [SerializeField] private Transform startPointObject;
        [SerializeField] private Transform downPointObject;
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private float timeFall = 1f;

        [SerializeField] private GameObject packageIfTriggerEnter;

        public void ObjectFall(GameObject prefab)
        {
            GameObject currentFallObject = Instantiate(prefab, startPointObject.position, prefab.transform.rotation);

            Collider colliderCurrentFallObject;
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
            }
            else
            {
                Debug.LogError("�� ������� � ����� ��� ������� ��������, �������� MovePointToPoint");
            }
        }
        IEnumerator ParticleFall(float t)
        {
            yield return new WaitForSeconds(t);
            particleSystem.Play();
            yield return new WaitForSeconds(0.2f);
            particleSystem.Stop();
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
