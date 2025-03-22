using DG.Tweening;
using External.DI;
using External.Storage;
using Game.Environment.Item;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Environment.LMixTable
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class Workbench : MonoBehaviour, ILeftMouseDownClickable
    {
        [SerializeField] private TriggerObject triggerObject;
        [SerializeField] private ParticleSystem particleWorkbench;
        [SerializeField] private GameObject prefabDragPoint;
        [SerializeField] private GameObject content;
        [SerializeField] private Transform mixTable;
        [Header("Buttons")]
        [SerializeField] private MixButton mixButton;
        [SerializeField] private PickUpButton pickUpButton;
        [SerializeField] private ClearButton clearButton;
        [Header("Recieps Craft")]
        [SerializeField] private List<CraftRecipe> recipes = new List<CraftRecipe>();
        [Header("SoundsCraftCells")]
        [SerializeField] private List<AudioClip> soundsCraftCells = new List<AudioClip>();

        public float timeIngradientMoveToMixTable = 0.5f;
        public float timeIngradientPickUpPlayer = 1.5f;
        public float timeNewIngradientCreation = 1f;

        public UnityEvent OnStartWorkbenchOpen;
        public UnityEvent OnEndWorkbenchOpen;

        public UnityEvent OnStartWorkbenchClose;
        public UnityEvent OnEndWorkbenchClose;

        public UnityEvent OnMixIngradients;
        public UnityEvent OnClearIngradients;
        public UnityEvent<IngradientData> OnCreatIngradient;
        public UnityEvent<PickUpItem> OnCreatPickUpItem;

        public UnityEvent<IngradientData> OnDragIngradient;
        public UnityEvent<IngradientData> OnDropIngradient;
        private Player player;
        private PlayerMouseMove playerMouseMove;
        private GameBootstrap gameBootstrap;

        private Collider colliderWorkbench;
        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;

        public bool IsOpen => isOpen;
        private bool isOpen = false;

        public bool IsDrag => isDrag;
        private bool isDrag = false;

        public bool IsEndDrag => isEndDrag;
        private bool isEndDrag = false;

        private bool isClick = false;

        private List<IngradientDragCell> ingradientDragCellsInMixTable = new List<IngradientDragCell>();
        private List<IngradientDragObject> ingradientDragObjectsInMixTable = new List<IngradientDragObject>();
        private List<IngradientSpawner> ingradientSpawners = new List<IngradientSpawner>();

        private IngradientDragBase pointer;

        public const int countIngradiensTaken = 1;


        public void Init(Player player, PlayerMouseMove playerMouseMove, GameBootstrap gameBootstrap)
        {
            this.gameBootstrap = gameBootstrap;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            colliderWorkbench = GetComponent<Collider>();
            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            print(mixTable.GetComponent<Collider>().bounds.max.x);
            print(mixTable.GetComponent<Collider>().bounds.min.x);

            if (mixButton == null)
                Debug.LogError("Ошибка. Не добавлен MixButton");
            if (pickUpButton == null)
                Debug.LogError("Ошибка. Не добавлен PickUpButton");
            if (clearButton == null)
                Debug.LogError("Ошибка. Не добавлен ClearButton");

            mixButton.Init(this, gameBootstrap);
            pickUpButton.Init(this, gameBootstrap);
            clearButton.Init(this, gameBootstrap);

            mixButton.SetActive(false);
            pickUpButton.SetActive(false);
            clearButton.SetActive(true);

            for (int i = 0; i < content.transform.childCount; i++)
            {
                if (content.transform.GetChild(i).TryGetComponent<IngradientSpawner>(out var ingradientSpawner))
                    ingradientSpawners.Add(ingradientSpawner);
            }

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

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (isClick)
                {
                    isClick = false;

                    for (int i = 0; i < ingradientSpawners.Count; i++)
                    {
                        if (ingradientSpawners[i].GetIngradient().typeIngradient == player.GetPickUpItem().NameItem)
                        {
                            IngradientData ingradient = new IngradientData();
                            ingradient.countIngradient = 1;
                            ingradient.typeIngradient = player.GetPickUpItem().NameItem;

                            Destroy(player.PutItem().gameObject);

                            AddIngradient(ingradient);
                            break;
                        }
                    }
                }
            });

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

            OnMixIngradients.AddListener(() =>
            {
                gameBootstrap.OnPlayOneShotRandomSound(soundsCraftCells);
            });

            Debug.Log("Workbench: Успешно иницилизирован");

        }

        public void OnUpdate(float deltaTime)
        {
            if (openObject != null)
                openObject.OnUpdate(deltaTime);
            mixButton.OnUpdate(deltaTime);
            pickUpButton.OnUpdate(deltaTime);
            clearButton.OnUpdate(deltaTime);
        }

        public static void ReplaceIngradientData(IngradientData ingradientData)
        {
            for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.ingradients.Count; i++)
            {
                if (SaveManager.playerDatabase.JSONPlayer.resources.ingradients[i].typeIngradient == ingradientData.typeIngradient)
                    SaveManager.playerDatabase.JSONPlayer.resources.ingradients[i] = ingradientData;
            }

            SaveManager.UpdatePlayerDatabase();
        }

        /// <summary>
        /// Функция возращает pointer обьект
        /// </summary>
        /// <param name="ingradientSpawner"></param>
        /// <returns></returns>
        public IngradientDragBase DragIngradient(IngradientSpawner ingradientSpawner)
        {
            this.pointer = InstantiatePointer(ingradientSpawner);
            if (ingradientSpawner.GetIngradient().countIngradient > 0)
            {
                isDrag = true;
                OnDragIngradient?.Invoke(ingradientSpawner.GetIngradient());

                for (int i = 0; i < ingradientDragCellsInMixTable.Count; i++)
                {
                    ingradientDragCellsInMixTable[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                }

                pointer.GetComponentInChildren<SpriteRenderer>().sortingOrder = ingradientDragCellsInMixTable.Count + 1;

                StartCoroutine(IColliderDragCellsEnabled(0f, false));
                StartCoroutine(IColliderSpawnersEnabled(0f, false));
            }

            return pointer;
        }

        public void DragIngradient(IngradientDragBase ingradientDragBase)
        {
            this.pointer = ingradientDragBase;

            IngradientData ingradient = new IngradientData();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientDragBase.GetNameIngradient();

            for (int i = 0; i < ingradientDragCellsInMixTable.Count; i++)
            {
                ingradientDragCellsInMixTable[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            }

            pointer.GetComponentInChildren<SpriteRenderer>().sortingOrder = ingradientDragCellsInMixTable.Count + 1;

            StartCoroutine(IColliderDragCellsEnabled(0f, false));
            StartCoroutine(IColliderSpawnersEnabled(0f, false));

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
            IngradientData ingradient = ingradientSpawner.PickUpIngradient(countIngradiensTaken);

            isEndDrag = true;

            if (isDrag && ingradient != null)
            {
                if (InMixTable())
                {
                    if (!ingradientDragCellsInMixTable.Exists(x => x == pointer))
                        ingradientDragCellsInMixTable.Add((IngradientDragCell)pointer);

                    for (int i = 0; i < ingradientDragCellsInMixTable.Count; i++)
                    {
                        ingradientDragCellsInMixTable[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                    }

                    pointer.GetComponentInChildren<SpriteRenderer>().sortingOrder = ingradientDragCellsInMixTable.Count + 1;

                    pointer = null;
                }
                else
                {
                    ingradientSpawner.PutIngradient(countIngradiensTaken);

                    Destroy(pointer.gameObject);

                    pointer = null;
                }


                pickUpButton.SetActive(CheckPickUpItem());

                if (CheckIngradientsInMixTable())
                    mixButton.SetActive(true);
                else
                    mixButton.SetActive(false);
            }
            else
            {
                pointer = InstantiatePointer(ingradientSpawner);
                pointer.transform.position = ingradientSpawner.transform.position;
                Bounds boundsIngradient = pointer.GetComponent<Collider>().bounds;
                Bounds boundsMixTable = mixTable.GetComponent<Collider>().bounds;

                float x = UnityEngine.Random.Range(boundsMixTable.min.x + (boundsIngradient.size.x / 2), boundsMixTable.max.x - (boundsIngradient.size.x / 2));
                float z = UnityEngine.Random.Range(boundsMixTable.min.z + (boundsIngradient.size.z / 2), boundsMixTable.max.z - (boundsIngradient.size.z / 2));

                pointer.transform.DOMove(new Vector3(x, pointer.transform.position.y, z), timeIngradientMoveToMixTable).SetEase(Ease.Linear);

                if (!ingradientDragCellsInMixTable.Exists(x => x == pointer))
                    ingradientDragCellsInMixTable.Add((IngradientDragCell)pointer);

                for (int i = 0; i < ingradientDragCellsInMixTable.Count; i++)
                {
                    ingradientDragCellsInMixTable[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                }

                pointer.GetComponentInChildren<SpriteRenderer>().sortingOrder = ingradientDragCellsInMixTable.Count + 1;
                StartCoroutine(IDropFromIngradientSpawner(pointer.GetComponent<IngradientDragCell>(), ingradientSpawner, timeIngradientMoveToMixTable));

                pointer = null;
            }

            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            StartCoroutine(IEndDrag(0.1f));
            StartCoroutine(IColliderDragObjectsEnabled(0.1f, true));
            StartCoroutine(IColliderDragCellsEnabled(0.1f, true));
            StartCoroutine(IColliderSpawnersEnabledWithCondition(0.1f, true));
        }

        public void DropIngradient(IngradientDragCell ingradientCell)
        {
            IngradientData ingradient = new IngradientData();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientCell.GetNameIngradient();

            isEndDrag = true;

            if (isDrag)
            {
                if (InMixTable())
                {
                    if (!ingradientDragCellsInMixTable.Exists(x => x == pointer))
                        ingradientDragCellsInMixTable.Add(ingradientCell);
                }
                else
                {
                    IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetNameIngradient());
                    ingradientSpawner.PutIngradient(countIngradiensTaken);

                    ingradientDragCellsInMixTable.Remove((IngradientDragCell)pointer);

                    Destroy(ingradientCell.gameObject);
                }
            }
            else
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetNameIngradient());
                ingradientCell.transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                ingradientDragCellsInMixTable.Remove(ingradientCell);
                StartCoroutine(IDropToIngradientSpawner(ingradientCell, ingradientSpawner, timeIngradientMoveToMixTable));
            }

            pointer = null;
            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);


            pickUpButton.SetActive(CheckPickUpItem());

            if (CheckIngradientsInMixTable())
                mixButton.SetActive(true);
            else
                mixButton.SetActive(false);

            StartCoroutine(IEndDrag(0.1f));
            StartCoroutine(IColliderDragObjectsEnabled(0.1f, true));
            StartCoroutine(IColliderDragCellsEnabled(0.1f, true));
            StartCoroutine(IColliderSpawnersEnabledWithCondition(0.1f, true));
        }

        public void DropIngradient(IngradientDragObject ingradientDragObject)
        {
            IngradientData ingradient = new IngradientData();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientDragObject.GetNameIngradient();

            isEndDrag = true;

            if (isDrag)
            {
                if (InMixTable())
                {
                    if (!ingradientDragObjectsInMixTable.Exists(x => x == pointer))
                        ingradientDragObjectsInMixTable.Add(ingradientDragObject);
                }
                else
                    ingradientDragObject.transform.DOMove(mixTable.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
            }

            pointer = null;
            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            mixButton.SetActive(false);

            StartCoroutine(IEndDrag(0.05f));
            StartCoroutine(IColliderDragObjectsEnabled(0.1f, true));
            StartCoroutine(IColliderDragCellsEnabled(0.1f, true));
            StartCoroutine(IColliderSpawnersEnabledWithCondition(0.1f, true));
        }

        private IEnumerator IEndDrag(float time)
        {
            yield return new WaitForSeconds(time);
            isEndDrag = false;
        }

        private IEnumerator IColliderDragCellsEnabled(float time, bool isActive)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < ingradientDragCellsInMixTable.Count; i++)
            {
                ingradientDragCellsInMixTable[i].GetComponent<Collider>().enabled = isActive;
            }
        }

        private IEnumerator IColliderDragObjectsEnabled(float time, bool isActive)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < ingradientDragObjectsInMixTable.Count; i++)
            {
                ingradientDragObjectsInMixTable[i].GetComponent<Collider>().enabled = isActive;
            }

            pickUpButton.SetActive(CheckPickUpItem());
        }

        private IEnumerator IColliderSpawnersEnabled(float time, bool isActive)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                ingradientSpawners[i].GetComponent<Collider>().enabled = isActive;
            }
        }

        private IEnumerator IColliderSpawnersEnabledWithCondition(float time, bool isActive)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                if (ingradientSpawners[i].GetIngradient().countIngradient > 0)
                    ingradientSpawners[i].GetComponent<Collider>().enabled = isActive;
            }
        }

        private IEnumerator IDropToIngradientSpawner(IngradientDragCell ingradientCell, IngradientSpawner ingradientSpawner, float time)
        {
            yield return new WaitForSeconds(time);
            ingradientSpawner.PutIngradient(countIngradiensTaken);

            if (ingradientCell != null)
            {
                Destroy(ingradientCell.gameObject);
            }
        }

        private IEnumerator IDropFromIngradientSpawner(IngradientDragCell ingradientCell, IngradientSpawner ingradientSpawner, float time)
        {
            yield return new WaitForSeconds(time);

            pickUpButton.SetActive(CheckPickUpItem());

            if (CheckIngradientsInMixTable())
                mixButton.SetActive(true);
            else
                mixButton.SetActive(false);
        }

        /// <summary>
        /// Функция создает pointer обьект
        /// </summary>
        /// <param name="ingradientSpawner"></param>
        /// <returns></returns>
        public IngradientDragBase InstantiatePointer(IngradientSpawner ingradientSpawner)
        {
            IngradientDragBase pointer = Instantiate(prefabDragPoint, content.transform).GetComponent<IngradientDragBase>();
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

        public void PickUpItem()
        {
            if (ingradientDragObjectsInMixTable.Count > 0 && !player.PlayerPickUpItem)
            {
                IngradientDragObject ingradientDragObject = ingradientDragObjectsInMixTable[0];
                ingradientDragObjectsInMixTable.Remove(ingradientDragObject);

                ingradientDragObject.GetComponent<Rigidbody>().isKinematic = false;
                PickUpItem pickUpItem = ingradientDragObject.GetComponent<PickUpItem>();
                pickUpItem.enabled = true;
                pickUpItem.GetComponent<Collider>().enabled = false;

                ingradientDragObject.transform.DOMove(player.transform.position, timeIngradientPickUpPlayer).SetEase(Ease.Linear);

                pickUpButton.SetActive(false);

                StartCoroutine(IPickUpIngradientDragObject(ingradientDragObject, timeIngradientPickUpPlayer));
            }
            else if (ingradientDragCellsInMixTable.Count == 1 && !player.PlayerPickUpItem)
            {
                IngradientDragCell ingradientDragCell = ingradientDragCellsInMixTable[0];
                ingradientDragCellsInMixTable.Remove(ingradientDragCell);

                PickUpItem pickUpItem = Instantiate(GameBootstrap.FindPickUpItemToPrefabs(ingradientDragCell.GetNameIngradient())).GetComponent<PickUpItem>();
                pickUpItem.GetComponent<Rigidbody>().isKinematic = false;
                pickUpItem.enabled = true;
                pickUpItem.GetComponent<Collider>().enabled = false;
                pickUpItem.name = ingradientDragCell.GetNameIngradient();
                pickUpItem.NameItem = ingradientDragCell.GetNameIngradient();

                ingradientDragCell.transform.DOMove(player.transform.position, timeIngradientPickUpPlayer).SetEase(Ease.Linear);
                pickUpButton.SetActive(false);

                StartCoroutine(IPickUpIngradientPickUpItem(pickUpItem, ingradientDragCell, timeIngradientPickUpPlayer));
            }
        }

        public bool CheckPickUpItem()
        {
            return ((ingradientDragCellsInMixTable.Count == 1 && ingradientDragObjectsInMixTable.Count == 0) ||
                (ingradientDragCellsInMixTable.Count == 0 && ingradientDragObjectsInMixTable.Count == 1)) && !player.PlayerPickUpItem;
        }

        private IEnumerator IPickUpIngradientDragObject(IngradientDragObject ingradientDragObject, float time)
        {
            yield return new WaitForSeconds(time);

            ingradientDragObject.GetComponent<Collider>().enabled = true;

            player.PickUpItem(ingradientDragObject.GetComponent<PickUpItem>());

            Destroy(ingradientDragObject);
        }

        private IEnumerator IPickUpIngradientPickUpItem(PickUpItem pickUpItem, IngradientDragCell ingradientDragCell, float time)
        {
            yield return new WaitForSeconds(time);

            pickUpItem.GetComponent<Collider>().enabled = true;

            Destroy(ingradientDragCell.gameObject);

            player.PickUpItem(pickUpItem.GetComponent<PickUpItem>());
        }

        public void MixIngradients()
        {
            if (CheckIngradientsInMixTable(out List<IngradientData> outIngradients, out List<PickUpItem> outPickUpItems))
            {
                print("Инградиенты успешно смешиваются");
                particleWorkbench.Play();

                for (int j = 0; j < ingradientDragCellsInMixTable.Count; j++)
                {
                    ingradientDragCellsInMixTable[j].GetComponent<Collider>().enabled = false;
                    ingradientDragCellsInMixTable[j].transform.DOMove(mixTable.transform.position, timeNewIngradientCreation).SetEase(Ease.Linear);
                    ingradientDragCellsInMixTable[j].transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), timeNewIngradientCreation).SetEase(Ease.Linear);
                    ingradientDragCellsInMixTable[j].transform.DORotate(new Vector3(0, 0, 1440), timeNewIngradientCreation, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                }

                StartCoroutine(IAnimationDeleteIngradients(timeNewIngradientCreation, outIngradients, outPickUpItems));
                OnMixIngradients?.Invoke();

                mixButton.SetActive(false);
            }
        }

        private IEnumerator IAnimationDeleteIngradients(float time, List<IngradientData> outIngradients, List<PickUpItem> outPickUpItems)
        {
            yield return new WaitForSeconds(time);

            for (int i = 0; i < ingradientDragCellsInMixTable.Count; i++)
            {
                Destroy(ingradientDragCellsInMixTable[i].gameObject);
            }

            ingradientDragCellsInMixTable.Clear();


            for (int i = 0; i < outIngradients.Count; i++)
            {
                OnCreatIngradient?.Invoke(outIngradients[i]);
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(outIngradients[i].typeIngradient);
                IngradientDragBase ingradientCell = InstantiatePointer(ingradientSpawner);

                ingradientCell.GetComponent<Collider>().enabled = false;
                ingradientCell.transform.position = new Vector3(mixTable.position.x, ingradientSpawner.transform.position.y, mixTable.position.z);

                Bounds boundsIngradient = ingradientCell.GetComponent<Collider>().bounds;
                Bounds boundsMixTable = mixTable.GetComponent<Collider>().bounds;

                float x = UnityEngine.Random.Range(boundsMixTable.min.x + (boundsIngradient.size.x / 2), boundsMixTable.max.x - (boundsIngradient.size.x / 2));
                float z = UnityEngine.Random.Range(boundsMixTable.min.z + (boundsIngradient.size.z / 2), boundsMixTable.max.z - (boundsIngradient.size.z / 2));

                ingradientCell.transform.DOMove(new Vector3(x, ingradientCell.transform.position.y, z), timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                ingradientCell.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                ingradientCell.transform.DOScale(prefabDragPoint.transform.localScale, timeNewIngradientCreation).SetEase(Ease.Linear);
                ingradientCell.transform.DORotate(new Vector3(90, 0, 1440), timeNewIngradientCreation, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                ingradientDragCellsInMixTable.Add((IngradientDragCell)ingradientCell);
            }

            for (int i = 0; i < outPickUpItems.Count; i++)
            {
                OnCreatPickUpItem?.Invoke(outPickUpItems[i]);
                PickUpItem pickUpItem = Instantiate(outPickUpItems[i], mixTable.transform);
                pickUpItem.GetComponent<Rigidbody>().isKinematic = true;
                pickUpItem.GetComponent<PickUpItem>().enabled = false;

                IngradientDragObject ingradientDragObject = pickUpItem.AddComponent<IngradientDragObject>();
                IngradientData ingradient = new IngradientData();
                ingradient.countIngradient = 1;
                ingradientDragObject.Init(ingradient, this, mixTable.GetComponent<Collider>());

                ingradientDragObject.GetComponent<Collider>().enabled = false;
                ingradientDragObject.transform.position = new Vector3(mixTable.position.x, ingradientSpawners[0].transform.position.y, mixTable.position.z);

                Bounds boundsIngradient = ingradientDragObject.GetComponent<Collider>().bounds;
                Bounds boundsMixTable = mixTable.GetComponent<Collider>().bounds;

                float x = UnityEngine.Random.Range(boundsMixTable.min.x + (boundsIngradient.size.x / 2), boundsMixTable.max.x - (boundsIngradient.size.x / 2));
                float z = UnityEngine.Random.Range(boundsMixTable.min.z + (boundsIngradient.size.z / 2), boundsMixTable.max.z - (boundsIngradient.size.z / 2));

                ingradientDragObject.transform.DOMove(new Vector3(x, ingradientDragObject.transform.position.y, z), timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                ingradientDragObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                ingradientDragObject.transform.DOScale(prefabDragPoint.transform.localScale, timeNewIngradientCreation).SetEase(Ease.Linear);
                ingradientDragObject.transform.DORotate(new Vector3(90, 0, 1440), timeNewIngradientCreation, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                ingradientDragObjectsInMixTable.Add(ingradientDragObject);
            }

            StartCoroutine(IColliderDragObjectsEnabled(time, true));
            StartCoroutine(IColliderDragCellsEnabled(time, true));
            StartCoroutine(IAnimationCreateIngradient(time));

        }

        private IEnumerator IAnimationCreateIngradient(float time)
        {
            yield return new WaitForSeconds(time);
            particleWorkbench.Stop();
        }

        public bool CheckIngradientsInMixTable()
        {
            return CheckIngradientsInMixTable(out List<IngradientData> outIngradients, out List<PickUpItem> outPickUpItems);
        }

        public bool CheckIngradientsInMixTable(out List<IngradientData> outIngradients, out List<PickUpItem> outPickUpItems)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                List<IngradientData> ingradientInputCells = new List<IngradientData>();
                List<IngradientData> ingradientInput = new List<IngradientData>();

                for (int j = 0; j < ingradientDragCellsInMixTable.Count; j++)
                {
                    if (!ingradientInputCells.Exists(x => x.typeIngradient == ingradientDragCellsInMixTable[j].GetNameIngradient()))
                    {
                        IngradientData ingradient = new IngradientData();
                        ingradient.countIngradient = countIngradiensTaken;
                        ingradient.typeIngradient = ingradientDragCellsInMixTable[j].GetNameIngradient();
                        ingradientInputCells.Add(ingradient);
                    }
                    else
                    {
                        for (int k = 0; k < ingradientInputCells.Count; k++)
                        {
                            if (ingradientInputCells[k].typeIngradient == ingradientDragCellsInMixTable[j].GetNameIngradient())
                                ingradientInputCells[k].countIngradient += countIngradiensTaken;
                        }
                    }
                }

                for (int j = 0; j < recipes[i].inputIngradients.Count; j++)
                {
                    if (!ingradientInput.Exists(x => x.typeIngradient == recipes[i].inputIngradients[j].typeIngradient))
                    {
                        IngradientData ingradient = new IngradientData();
                        ingradient.countIngradient = recipes[i].inputIngradients[j].countIngradient;
                        ingradient.typeIngradient = recipes[i].inputIngradients[j].typeIngradient;
                        ingradientInput.Add(ingradient);
                    }
                    else
                    {
                        for (int k = 0; k < ingradientInput.Count; k++)
                        {
                            if (ingradientInput[k].typeIngradient == recipes[i].inputIngradients[j].typeIngradient)
                                ingradientInput[k].countIngradient += recipes[i].inputIngradients[j].countIngradient;
                        }
                    }
                }

                ingradientInputCells = ingradientInputCells.OrderBy(x => x.typeIngradient).ToList();
                ingradientInput = ingradientInput.OrderBy(x => x.typeIngradient).ToList();

                if (CheckIngradients(ingradientInput, ingradientInputCells))
                {
                    outIngradients = recipes[i].outIngradients;
                    outPickUpItems = recipes[i].outPickUpItems;
                    return true;
                }
            }

            outIngradients = null;
            outPickUpItems = null;
            return false;
        }

        private bool CheckIngradients(List<IngradientData> ingradientInput, List<IngradientData> ingradientInputCells)
        {
            if (ingradientInput.Count == ingradientInputCells.Count)
            {
                for (int j = 0; j < ingradientInput.Count; j++)
                {
                    if (ingradientInputCells[j].typeIngradient != ingradientInput[j].typeIngradient || ingradientInputCells[j].countIngradient != ingradientInput[j].countIngradient)
                        return false;
                }

                return true;
            }
            else
                return false;
        }

        public void ClearIngredients()
        {
            for (int i = 0; i < ingradientDragCellsInMixTable.Count; i++)
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientDragCellsInMixTable[i].GetNameIngradient());
                ingradientDragCellsInMixTable[i].transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                StartCoroutine(IClearIngradients(ingradientSpawner, ingradientDragCellsInMixTable[i], timeIngradientMoveToMixTable));
            }

            mixButton.SetActive(false);
        }

        private IEnumerator IClearIngradients(IngradientSpawner ingradientSpawner, IngradientDragCell ingradientCell, float time)
        {
            yield return new WaitForSeconds(time);
            ingradientDragCellsInMixTable.Remove(ingradientCell);
            ingradientSpawner.PutIngradient(countIngradiensTaken);
            Destroy(ingradientCell.gameObject);
        }




        public void AddIngradient(IngradientData ingradient)
        {
            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                if (ingradientSpawners[i].GetIngradient().typeIngradient == ingradient.typeIngradient)
                {
                    ingradientSpawners[i].PutIngradient(1);
                    return;
                }
            }
        }

        public void OnMouseLeftClickDownObject()
        {
            if (player.PlayerPickUpItem && !IsOpen)
                isClick = true;
        }

        public void OnMouseLeftClickDownOtherObject()
        {
            isClick = false;
        }
    }
}
