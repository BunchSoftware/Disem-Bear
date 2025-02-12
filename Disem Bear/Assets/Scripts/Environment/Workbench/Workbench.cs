using DG.Tweening;
using External.Storage;
using Game.Environment.Item;
using Game.Environment.LModelBoard;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


namespace Game.Environment.LMixTable
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class Workbench : MonoBehaviour
    {
        [SerializeField] private TriggerObject triggerObject;
        [SerializeField] private GameObject prefabDragPoint;
        [SerializeField] private GameObject content;
        [SerializeField] private List<IngradientSpawner> ingradientSpawners;
        [SerializeField] private Transform mixTable;

        public float timeIngradientMoveToMixTable = 0.5f;

        public UnityEvent OnStartWorkbenchOpen;
        public UnityEvent OnEndWorkbenchOpen;

        public UnityEvent OnStartWorkbenchClose;
        public UnityEvent OnEndWorkbenchClose;

        public UnityEvent<Ingradient> OnDragIngradient;
        public UnityEvent<Ingradient> OnDropIngradient;

        private SaveManager saveManager;
        private Player player;
        private PlayerMouseMove playerMouseMove;

        private Collider colliderWorkbench;
        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;

        public bool IsOpen => isOpen;
        private bool isOpen = false;

        public bool IsDrag => isDrag;
        private bool isDrag = false;

        private IngradientCell pointer;

        public const int countIngradiensTaken = 1;

        public void Init(SaveManager saveManager, Player player, PlayerMouseMove playerMouseMove)
        {
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            colliderWorkbench = GetComponent<Collider>();
            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                if (ingradientSpawners[i] == null)
                    Debug.LogError("Ошибка. Не добавлен Ingradient Spawner в Workbench");
                ingradientSpawners[i].Init(this, triggerObject);
            }

            content.GetComponent<Collider>().enabled = false;
            for (int i = 0; i < content.transform.childCount; i++)
            {
                content.transform.GetChild(i).GetComponent<Collider>().enabled = false;
            }

            openObject.OnStartObjectOpen.AddListener(() =>
            {
                scaleChooseObject.on = false;
                colliderWorkbench.enabled = false;
                OnStartWorkbenchOpen?.Invoke();
            });
            openObject.OnEndObjectOpen.AddListener(() =>
            {
                isOpen = true;
                OnEndWorkbenchOpen?.Invoke();

                content.GetComponent<Collider>().enabled = true;
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    content.transform.GetChild(i).GetComponent<Collider>().enabled = true;
                }
            });
            openObject.OnStartObjectClose.AddListener(() =>
            {
                OnStartWorkbenchClose?.Invoke();
                isOpen = false;

                content.GetComponent<Collider>().enabled = false;
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    content.transform.GetChild(i).GetComponent<Collider>().enabled = false;
                }
            });
            openObject.OnEndObjectClose.AddListener(() =>
            {
                colliderWorkbench.enabled = true;
                scaleChooseObject.on = true;
                OnEndWorkbenchClose?.Invoke();
            });
            openObject.Init(triggerObject, playerMouseMove, player);
        }

        /// <summary>
        /// Функция возращает pointer обьект
        /// </summary>
        /// <param name="ingradientSpawner"></param>
        /// <returns></returns>
        public IngradientCell DragIngradient(IngradientSpawner ingradientSpawner)
        {
            this.pointer = InstantiatePointer(ingradientSpawner);
            if (ingradientSpawner.GetIngradient().countIngradient > 0)
            {
                isDrag = true;
                OnDragIngradient?.Invoke(ingradientSpawner.GetIngradient());

                for (int i = 0; i < ingradientSpawners.Count; i++)
                {
                    ingradientSpawners[i].GetComponent<Collider>().enabled = false;
                }
            }

            return pointer;
        }

        public void DragIngradient(IngradientCell ingradientCell)
        {
            this.pointer = ingradientCell;

            Ingradient ingradient = new Ingradient();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientCell.GetTypeIngradient();

            isDrag = true;
            OnDragIngradient?.Invoke(ingradient);
        }

        private bool InMixTable()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;

            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxDistance);

            for (int i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].collider.gameObject.transform == mixTable)
                {
                    return true;
                }
            }

            return false;
        }

        public void DropIngradient(IngradientSpawner ingradientSpawner)
        {
            Ingradient ingradient = ingradientSpawner.PickUpIngradient(countIngradiensTaken);
            if (pointer != null && ingradient != null)
            {
                if(InMixTable() && isDrag)
                {
                    ingradientSpawner.EndDrag();
                    pointer.EndDrag();
                    pointer = null;
                }
                else
                {
                    if (pointer != null && isDrag)
                    {
                        ingradientSpawner.PutIngradient(countIngradiensTaken);
                        ingradientSpawner.EndDrag();
                        Destroy(pointer.gameObject);

                        pointer = null;
                    }
                }
            }
            else
            {
                pointer = InstantiatePointer(ingradientSpawner);
                pointer.transform.position = ingradientSpawner.transform.position;
                Bounds boundsIngradient = pointer.GetComponent<Collider>().bounds;
                Bounds boundsMixTable = mixTable.GetComponent<Collider>().bounds;

                float x = Random.Range(boundsMixTable.min.x + boundsIngradient.size.x / 2, boundsMixTable.max.x - boundsIngradient.size.x / 2);
                float z = Random.Range(boundsMixTable.min.z + boundsIngradient.size.z / 2, boundsMixTable.max.z - boundsIngradient.size.z / 2);

                pointer.transform.DOMove(new Vector3(x, pointer.transform.position.y, z), timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                pointer = null;
            }

            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                if (ingradientSpawners[i].GetIngradient().countIngradient > 0)
                    ingradientSpawners[i].GetComponent<Collider>().enabled = true;
            }
        }

        public void DropIngradient(IngradientCell ingradientCell)
        {
            Ingradient ingradient = new Ingradient();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientCell.GetTypeIngradient();

            pointer = ingradientCell;

            if (isDrag)
            {
                if (InMixTable())
                {
                    ingradientCell.EndDrag();
                    pointer = null;
                }
                else
                {
                    IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetTypeIngradient());
                    ingradientSpawner.PutIngradient(countIngradiensTaken);
                    ingradientCell.EndDrag();
                    Destroy(pointer.gameObject);

                    pointer = null;
                }
            }
            else
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetTypeIngradient());
                pointer.transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                StartCoroutine(IDragIngradient(ingradientSpawner, timeIngradientMoveToMixTable));
            }

            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                if (ingradientSpawners[i].GetIngradient().countIngradient > 0)
                    ingradientSpawners[i].GetComponent<Collider>().enabled = true;
            }
        }

        private IEnumerator IDragIngradient(IngradientSpawner ingradientSpawner, float time)
        {
            yield return new WaitForSeconds(time);
            ingradientSpawner.PutIngradient(countIngradiensTaken);
            pointer = null;
        }

        /// <summary>
        /// Функция создает pointer обьект
        /// </summary>
        /// <param name="ingradientSpawner"></param>
        /// <returns></returns>
        public IngradientCell InstantiatePointer(IngradientSpawner ingradientSpawner)
        {
            IngradientCell pointer = Instantiate(prefabDragPoint,content.transform).GetComponent<IngradientCell>();
            SpriteRenderer spriteRenderer = pointer.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = ingradientSpawner.GetSpriteIngradient();

            pointer.Init(ingradientSpawner.GetIngradient(), this, content.GetComponent<Collider>());

            return pointer;
        }

        public IngradientSpawner GetIngradientSpawnerOfTypeIngradient(string typeIngradient) 
        { 
            IngradientSpawner ingradientSpawner = null;

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                if (ingradientSpawners[i].GetIngradient().typeIngradient == typeIngradient)
                    return ingradientSpawners[i];
            }

            return ingradientSpawner;
        }


        public void OnUpdate(float deltaTime)
        {
            openObject.OnUpdate(deltaTime);

            //if (GetComponent<OpenObject>().ObjectIsOpen && !MixTable.GetComponent<ThingsInTableMix>().MixTableOn)
            //{
            //    MixTable.GetComponent<ThingsInTableMix>().MixTableOn = true;
            //}
            //else if (!GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().ObjectIsOpen && Input.GetMouseButtonDown(1))
            //{
            //    MixTable.GetComponent<ThingsInTableMix>().MixTableOn = false;
            //    if (MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject != null)
            //    {
            //        //Player.GetComponent<Player>().currentPickObject = MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject;
            //        MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject.transform.parent = Player.transform;
            //        MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject = null;
            //       // Player.GetComponent<Player>().PickSomething();
            //    }
            //}
        }
    }
}
