using External.DI;
using Game.Environment.Item;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private ToastManager toastManager;
        [Header("SoundsPackageCrash")]
        [SerializeField] private List<AudioClip> soundsPackageCrash = new();
        [SerializeField] private List<AudioClip> soundsPackageFall = new();

        private bool itemFlies = false;

        private Queue<ObjectFallTask> objectFallTasks = new Queue<ObjectFallTask>();
        private const int MaxObjectFall = 3;
        public void Init(Player player, ExerciseManager exerciseManager, ToastManager toastManager, GameBootstrap gameBootstrap)
        {
            this.gameBootstrap = gameBootstrap;
            this.toastManager = toastManager;
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
            gameBootstrap.OnPlayOneShotRandomSound(soundsPackageFall);

            Collider colliderCurrentFallObject;
            Vector3 lastDownPointPosition = downPointObject.position;
            if (currentFallObject.TryGetComponent(out colliderCurrentFallObject))
            {
                downPointObject.position = new Vector3(downPointObject.position.x, downPointObject.position.y + (colliderCurrentFallObject.bounds.size.y / 2), downPointObject.position.z);
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
            if (itemFlies || objectFallTasks.Count >= 1)
            {
                ObjectFallTask objectFallTask = new ObjectFallTask();
                objectFallTask.objectFall = prefab;

                if (objectFallTasks.Count <= MaxObjectFall)
                {
                    objectFallTasks.Enqueue(objectFallTask);
                }
                else
                {
                    toastManager.ShowToast("Достигнуто максимальное количество предметов на выдачу");
                }
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

        private IEnumerator ParticleFall(float t)
        {
            yield return new WaitForSeconds(t);
            particleSystem.Play();
            yield return new WaitForSeconds(0.2f);
            particleSystem.Stop();
        }

        private IEnumerator TimeReturnDownPointPos(float t, Vector3 lastDownPointPosition)
        {
            yield return new WaitForSeconds(t);
            downPointObject.position = lastDownPointPosition;
        }

        private IEnumerator PostBoxGetPackage(float t, PickUpItem pickUpItem)
        {
            yield return new WaitForSeconds(t);
            gameBootstrap.OnPlayOneShotRandomSound(soundsPackageCrash);
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
