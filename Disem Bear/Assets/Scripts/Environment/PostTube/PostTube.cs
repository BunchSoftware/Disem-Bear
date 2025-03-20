using System;
using System.Collections;
using System.Collections.Generic;
using External.DI;
using Game.Environment.Item;
using Game.LPlayer;
using UI;
using UI.PlaneTablet;
using UI.PlaneTablet.Exercise;
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

    public class ObjectFallTask
    {
        public GameObject objectFall;
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

        private GameBootstrap gameBootstrap;
        [Header("SoundsPackageCrash")]
        [SerializeField] private List<AudioClip> soundsPackageCrash = new();

        private bool itemFlies = false;

        private Queue<ObjectFallTask> objectFallTasks = new Queue<ObjectFallTask>();

        public void Init(Player player, ExerciseManager exerciseManager, ToastManager toastManager, GameBootstrap gameBootstrap)
        {
            this.gameBootstrap = gameBootstrap;
            postBox.Init(player, exerciseManager, toastManager, gameBootstrap);

            postBox.OnPostBoxEmpty += () =>
            {
                if (objectFallTasks.Count > 0)
                {
                    ObjectFallTask objectFallTask = objectFallTasks.Dequeue();
                    ObjectFallAction(objectFallTask.objectFall);
                }
            };
        }

        private void ObjectFallAction(GameObject prefab)
        {

            GameObject currentFallObject = Instantiate(prefab, startPointObject.position, prefab.transform.rotation);

            Collider colliderCurrentFallObject;
            Vector3 lastDownPointPosition = downPointObject.position;
            if (currentFallObject.TryGetComponent(out colliderCurrentFallObject))
            {
                downPointObject.position = new Vector3(downPointObject.position.x, downPointObject.position.y + colliderCurrentFallObject.bounds.size.y / 2, downPointObject.position.z);
            }

            itemFlies = true;

            StartCoroutine(PostBoxGetPackage(timeFall, currentFallObject.GetComponent<PickUpItem>()));
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
                Debug.LogError("На объекте в трубе нет скрипта движения, например MovePointToPoint");
                downPointObject.position = lastDownPointPosition;
            }
        }

        public void ObjectFall(GameObject prefab)
        {
            if (postBox.ItemInBox())
            {
                ObjectFallTask objectFallTask = new ObjectFallTask();
                objectFallTask.objectFall = prefab;

                objectFallTasks.Enqueue(objectFallTask);
            }
            else
            {
                ObjectFallAction(prefab);
            }
        }
      
        public void ObjectFall(string typeObject)
        {
            for (int i = 0; i < postTubeObjects.Count; i++)
            {
                if (postTubeObjects[i].typeObject == typeObject)
                {
                    Debug.Log(typeObject);
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
        public void ObjectFall(ExerciseItem exerciseItem)
        {
            ObjectFall(exerciseItem.typeItem);
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
        IEnumerator PostBoxGetPackage(float t, PickUpItem pickUpItem)
        {
            yield return new WaitForSeconds(t);
            if (soundsPackageCrash.Count > 0)
            {
                gameBootstrap.OnPlayOneShotSound(soundsPackageCrash[(int)(Time.deltaTime % soundsPackageCrash.Count)]);
            }
            postBox.PutItemInBox(pickUpItem);
            itemFlies = false;
        }

        public bool IsItemFlies()
        {
            return itemFlies;
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
