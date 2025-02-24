using DG.Tweening;
using External.Storage;
using Game.Environment.Item;
using Game.Environment.LModelBoard;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

        public float timeIngradientMoveToMixTable = 0.5f;
        public float timeIngradientPickUpPlayer = 1.5f;
        public float timeNewIngradientCreation = 1f;

        public UnityEvent OnStartWorkbenchOpen;
        public UnityEvent OnEndWorkbenchOpen;

        public UnityEvent OnStartWorkbenchClose;
        public UnityEvent OnEndWorkbenchClose;

        public UnityEvent OnMixIngradients;
        public UnityEvent OnClearIngradients;

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

        public bool IsEndDrag => isEndDrag;
        private bool isEndDrag = false;

        private List<Ingradient> ingradients = new List<Ingradient>();
        private List<IngradientDragCell> ingradientDragCells = new List<IngradientDragCell>();
        private List<IngradientDragObject> ingradientDragObjects = new List<IngradientDragObject>();
        private List<IngradientSpawner> ingradientSpawners = new List<IngradientSpawner>();

        private IngradientDragBase pointer;

        public const int countIngradiensTaken = 1;

        public void Init(SaveManager saveManager, Player player, PlayerMouseMove playerMouseMove)
        {
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            colliderWorkbench = GetComponent<Collider>();
            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            mixButton.Init(this);
            pickUpButton.Init(this);
            clearButton.Init(this);

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
                ingradients.Add(ingradientSpawners[i].GetIngradient());
            }

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                if (ingradientSpawners[i] == null)
                    Debug.LogError("������. �� �������� Ingradient Spawner � Workbench");
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

            Debug.Log("Workbench: ������� ��������������");
        }

        /// <summary>
        /// ������� ��������� pointer ������
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

                for (int i = 0; i < ingradientDragCells.Count; i++)
                {
                    ingradientDragCells[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                }

                pointer.GetComponent<SpriteRenderer>().sortingOrder = ingradientDragCells.Count + 1;

                StartCoroutine(IColliderDragCellsEnabled(0f, false));
                StartCoroutine(IColliderSpawnersEnabled(0f, false));
            }

            return pointer;
        }

        public void DragIngradient(IngradientDragBase ingradientDragBase)
        {
            this.pointer = ingradientDragBase;

            Ingradient ingradient = new Ingradient();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientDragBase.GetTypeIngradient();

            for (int i = 0; i < ingradientDragCells.Count; i++)
            {
                ingradientDragCells[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            }

            pointer.GetComponent<SpriteRenderer>().sortingOrder = ingradientDragCells.Count + 1;

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
            Ingradient ingradient = ingradientSpawner.PickUpIngradient(countIngradiensTaken);

            isEndDrag = true;

            if (isDrag && ingradient != null)
            {
                if(InMixTable())
                {
                    if(!ingradientDragCells.Exists(x => x == pointer))
                        ingradientDragCells.Add((IngradientDragCell)pointer);

                    for (int i = 0; i < ingradientDragCells.Count; i++)
                    {
                        ingradientDragCells[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                    }

                    pointer.GetComponent<SpriteRenderer>().sortingOrder = ingradientDragCells.Count + 1;

                    pointer = null;
                }
                else
                {
                    ingradientSpawner.PutIngradient(countIngradiensTaken);

                    Destroy(pointer.gameObject);

                    pointer = null;
                }
            }
            else
            {
                pointer = InstantiatePointer(ingradientSpawner);
                pointer.transform.position = ingradientSpawner.transform.position;
                Bounds boundsIngradient = pointer.GetComponent<Collider>().bounds;
                Bounds boundsMixTable = mixTable.GetComponent<Collider>().bounds;

                float x = UnityEngine.Random.Range(boundsMixTable.min.x + boundsIngradient.size.x / 2, boundsMixTable.max.x - boundsIngradient.size.x / 2);
                float z = UnityEngine.Random.Range(boundsMixTable.min.z + boundsIngradient.size.z / 2, boundsMixTable.max.z - boundsIngradient.size.z / 2);

                pointer.transform.DOMove(new Vector3(x, pointer.transform.position.y, z), timeIngradientMoveToMixTable).SetEase(Ease.Linear);

                if (!ingradientDragCells.Exists(x => x == pointer))
                    ingradientDragCells.Add((IngradientDragCell)pointer);

                for (int i = 0; i < ingradientDragCells.Count; i++)
                {
                    ingradientDragCells[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                }

                pointer.GetComponent<SpriteRenderer>().sortingOrder = ingradientDragCells.Count + 1;

                pointer = null;
            }

            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            if (CheckIngradientsInMixTable())
                mixButton.SetActive(true);
            else
                mixButton.SetActive(false);

            if (ingradientDragCells.Count > 0)
                pickUpButton.SetActive(false);
            else if(ingradientDragObjects.Count > 0)
                pickUpButton.SetActive(true);

            StartCoroutine(IEndDrag(0.1f));
            StartCoroutine(IColliderDragObjectsEnabled(0.1f, true));
            StartCoroutine(IColliderDragCellsEnabled(0.1f, true));
            StartCoroutine(IColliderSpawnersEnabledWithCondition(0.1f, true));
        }

        public void DropIngradient(IngradientDragCell ingradientCell)
        {
            Ingradient ingradient = new Ingradient();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientCell.GetTypeIngradient();

            isEndDrag = true;

            if (isDrag)
            {
                if (InMixTable())
                {
                    if (!ingradientDragCells.Exists(x => x == pointer))
                        ingradientDragCells.Add(ingradientCell);
                }
                else
                {
                    IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetTypeIngradient());
                    ingradientSpawner.PutIngradient(countIngradiensTaken);

                    ingradientDragCells.Remove((IngradientDragCell)pointer);

                    Destroy(ingradientCell.gameObject);
                }
            }
            else
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetTypeIngradient());
                ingradientCell.transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                StartCoroutine(IDropIngradientDragCell(ingradientCell, ingradientSpawner, timeIngradientMoveToMixTable));
            }

            pointer = null;
            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            StartCoroutine(IEndDrag(0.1f));
            StartCoroutine(IColliderDragObjectsEnabled(0.1f, true));
            StartCoroutine(IColliderDragCellsEnabled(0.1f, true));
            StartCoroutine(IColliderSpawnersEnabledWithCondition(0.1f, true));
        }

        public void DropIngradient(IngradientDragObject ingradientDragObject)
        {
            Ingradient ingradient = new Ingradient();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientDragObject.GetTypeIngradient();

            isEndDrag = true;

            if (isDrag)
            {
                if (InMixTable())
                {
                    if (!ingradientDragObjects.Exists(x => x == pointer))
                        ingradientDragObjects.Add(ingradientDragObject);
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
            for (int i = 0; i < ingradientDragCells.Count; i++)
            {
                ingradientDragCells[i].GetComponent<Collider>().enabled = isActive;
            }
        }

        private IEnumerator IColliderDragObjectsEnabled(float time, bool isActive)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < ingradientDragObjects.Count; i++)
            {
                ingradientDragObjects[i].GetComponent<Collider>().enabled = isActive;
            }

            if(ingradientDragObjects.Count > 0)
                pickUpButton.SetActive(true);
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

        private IEnumerator IDropIngradientDragCell(IngradientDragCell ingradientCell, IngradientSpawner ingradientSpawner, float time)
        {
            yield return new WaitForSeconds(time);
            ingradientSpawner.PutIngradient(countIngradiensTaken);

            ingradientDragCells.Remove(ingradientCell);

            if(ingradientCell != null)
            {
                Destroy(ingradientCell.gameObject);
            }

            if (ingradientDragCells.Count > 0)
                pickUpButton.SetActive(false);
            else if (ingradientDragObjects.Count > 0)
                pickUpButton.SetActive(true);

            if (CheckIngradientsInMixTable())
                mixButton.SetActive(true);
            else
                mixButton.SetActive(false);
        }

        private IEnumerator IDropIngradientDragObject(IngradientDragObject ingradientDragObject, float time)
        {
            yield return new WaitForSeconds(time);

            ingradientDragObject.GetComponent<Collider>().enabled = true;

            player.PickUpItem(ingradientDragObject.GetComponent<PickUpItem>());

            Destroy(ingradientDragObject);
        }

        /// <summary>
        /// ������� ������� pointer ������
        /// </summary>
        /// <param name="ingradientSpawner"></param>
        /// <returns></returns>
        public IngradientDragBase InstantiatePointer(IngradientSpawner ingradientSpawner)
        {
            IngradientDragBase pointer = Instantiate(prefabDragPoint,content.transform).GetComponent<IngradientDragBase>();
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
            if(ingradientDragObjects.Count > 0 && !player.PlayerPickUpItem)
            {
                IngradientDragObject ingradientDragObject = ingradientDragObjects[0];
                ingradientDragObjects.Remove(ingradientDragObject);

                ingradientDragObject.GetComponent<Rigidbody>().isKinematic = false;
                PickUpItem pickUpItem = ingradientDragObject.GetComponent<PickUpItem>();
                pickUpItem.enabled = true;
                pickUpItem.GetComponent<Collider>().enabled = false;

                ingradientDragObject.transform.DOMove(player.transform.position, timeIngradientPickUpPlayer).SetEase(Ease.Linear);

                pickUpButton.SetActive(false);

                StartCoroutine(IDropIngradientDragObject(ingradientDragObject, timeIngradientPickUpPlayer));
            }
                
        }

        public void MixIngradients()
        {
            if (CheckIngradientsInMixTable(out List<Ingradient> outIngradients, out List<PickUpItem> outPickUpItems))
            {
                print("����������� ������� �����������");
                particleWorkbench.Play();

                for (int j = 0; j < ingradientDragCells.Count; j++)
                {
                    ingradientDragCells[j].GetComponent<Collider>().enabled = false;
                    ingradientDragCells[j].transform.DOMove(mixTable.transform.position, timeNewIngradientCreation).SetEase(Ease.Linear);
                    ingradientDragCells[j].transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), timeNewIngradientCreation).SetEase(Ease.Linear);
                    ingradientDragCells[j].transform.DORotate(new Vector3(0, 0, 1440), timeNewIngradientCreation, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                }

                StartCoroutine(IAnimationDeleteIngradients(timeNewIngradientCreation, outIngradients, outPickUpItems));
                OnMixIngradients?.Invoke();

                mixButton.SetActive(false);
            }
        }

        private IEnumerator IAnimationDeleteIngradients(float time, List<Ingradient> outIngradients, List<PickUpItem> outPickUpItems)
        {
            yield return new WaitForSeconds(time);

            for (int i = 0; i < ingradientDragCells.Count; i++)
            {
                Destroy(ingradientDragCells[i].gameObject);
            }

            ingradientDragCells.Clear();


            for (int i = 0; i < outIngradients.Count; i++)
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(outIngradients[i].typeIngradient);
                IngradientDragBase ingradientCell = InstantiatePointer(ingradientSpawner);

                ingradientCell.GetComponent<Collider>().enabled = false;
                ingradientCell.transform.position = new Vector3(mixTable.position.x, ingradientSpawner.transform.position.y, mixTable.position.z);

                Bounds boundsIngradient = ingradientCell.GetComponent<Collider>().bounds;
                Bounds boundsMixTable = mixTable.GetComponent<Collider>().bounds;

                float x = UnityEngine.Random.Range(boundsMixTable.min.x + boundsIngradient.size.x / 2, boundsMixTable.max.x - boundsIngradient.size.x / 2);
                float z = UnityEngine.Random.Range(boundsMixTable.min.z + boundsIngradient.size.z / 2, boundsMixTable.max.z - boundsIngradient.size.z / 2);

                ingradientCell.transform.DOMove(new Vector3(x, ingradientCell.transform.position.y, z), timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                ingradientCell.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                ingradientCell.transform.DOScale(prefabDragPoint.transform.localScale, timeNewIngradientCreation).SetEase(Ease.Linear);
                ingradientCell.transform.DORotate(new Vector3(90, 0, 1440), timeNewIngradientCreation, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                ingradientDragCells.Add((IngradientDragCell)ingradientCell);
            }

            for (int i = 0; i < outPickUpItems.Count; i++)
            {
                PickUpItem pickUpItem = Instantiate(outPickUpItems[i], content.transform);
                pickUpItem.GetComponent<Rigidbody>().isKinematic = true;
                pickUpItem.GetComponent<PickUpItem>().enabled = false;

                IngradientDragObject ingradientDragObject = pickUpItem.AddComponent<IngradientDragObject>();
                Ingradient ingradient = new Ingradient();
                ingradient.countIngradient = 1;
                ingradientDragObject.Init(ingradient, this, mixTable.GetComponent<Collider>());

                ingradientDragObject.GetComponent<Collider>().enabled = false;
                ingradientDragObject.transform.position = new Vector3(mixTable.position.x, ingradientSpawners[0].transform.position.y, mixTable.position.z);

                Bounds boundsIngradient = ingradientDragObject.GetComponent<Collider>().bounds;
                Bounds boundsMixTable = mixTable.GetComponent<Collider>().bounds;

                float x = UnityEngine.Random.Range(boundsMixTable.min.x + boundsIngradient.size.x / 2, boundsMixTable.max.x - boundsIngradient.size.x / 2);
                float z = UnityEngine.Random.Range(boundsMixTable.min.z + boundsIngradient.size.z / 2, boundsMixTable.max.z - boundsIngradient.size.z / 2);

                ingradientDragObject.transform.DOMove(new Vector3(x, ingradientDragObject.transform.position.y, z), timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                ingradientDragObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                ingradientDragObject.transform.DOScale(prefabDragPoint.transform.localScale, timeNewIngradientCreation).SetEase(Ease.Linear);
                ingradientDragObject.transform.DORotate(new Vector3(90, 0, 1440), timeNewIngradientCreation, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                ingradientDragObjects.Add(ingradientDragObject);
            }

            StartCoroutine(IColliderDragObjectsEnabled(time, true));
            StartCoroutine(IColliderDragCellsEnabled(time, true));
            StartCoroutine(IAnimationCreateIngradient(time));

        }
        IEnumerator IAnimationCreateIngradient(float time)
        {
            yield return new WaitForSeconds(time);
            particleWorkbench.Stop();         
        }

        public bool CheckIngradientsInMixTable()
        {
            return CheckIngradientsInMixTable(out List<Ingradient> outIngradients, out List<PickUpItem> outPickUpItems);
        }

        public bool CheckIngradientsInMixTable(out List<Ingradient> outIngradients, out List<PickUpItem> outPickUpItems)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                List<Ingradient> ingradientInputCells = new List<Ingradient>();
                List<Ingradient> ingradientInput = new List<Ingradient>();

                for (int j = 0; j < ingradientDragCells.Count; j++)
                {
                    if (!ingradientInputCells.Exists(x => x.typeIngradient == ingradientDragCells[j].GetTypeIngradient()))
                    {
                        Ingradient ingradient = new Ingradient();
                        ingradient.countIngradient = countIngradiensTaken;
                        ingradient.typeIngradient = ingradientDragCells[j].GetTypeIngradient();
                        ingradientInputCells.Add(ingradient);
                    }
                    else
                    {
                        for (int k = 0; k < ingradientInputCells.Count; k++)
                        {
                            if (ingradientInputCells[k].typeIngradient == ingradientDragCells[j].GetTypeIngradient())
                                ingradientInputCells[k].countIngradient += countIngradiensTaken;
                        }
                    }
                }

                for (int j = 0; j < recipes[i].inputIngradients.Count; j++)
                {
                    if (!ingradientInput.Exists(x => x.typeIngradient == recipes[i].inputIngradients[j].typeIngradient))
                    {
                        Ingradient ingradient = new Ingradient();
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

        private bool CheckIngradients(List<Ingradient> ingradientInput, List<Ingradient> ingradientInputCells)
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
            for (int i = 0; i < ingradientDragCells.Count; i++)
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientDragCells[i].GetTypeIngradient());
                ingradientDragCells[i].transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                StartCoroutine(IClearIngradients(ingradientSpawner, ingradientDragCells[i], timeIngradientMoveToMixTable));
            }

            mixButton.SetActive(false);
        }

        private IEnumerator IClearIngradients(IngradientSpawner ingradientSpawner, IngradientDragCell ingradientCell, float time)
        {
            yield return new WaitForSeconds(time);
            ingradientDragCells.Remove(ingradientCell);
            ingradientSpawner.PutIngradient(countIngradiensTaken);
            Destroy(ingradientCell.gameObject);
        }


        public void OnUpdate(float deltaTime)
        {
            openObject.OnUpdate(deltaTime);
        }

        public void AddIngradient(Ingradient ingradient)
        {
            for (int i = 0; i < ingradients.Count; i++)
            {
                if (ingradients[i].typeIngradient == ingradient.typeIngradient)
                {
                    ingradients[i].countIngradient += ingradient.countIngradient;
                    return;
                }
                else
                    ingradients.Add(ingradient);
            }
        }
    }
}
