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
        [SerializeField] private List<IngradientSpawner> ingradientSpawners;

        public float timeIngradientMoveToMixTable = 0.5f;
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

        private List<Ingradient> ingradients = new List<Ingradient>();
        private List<IngradientCell> ingradientCells = new List<IngradientCell>();
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

            mixButton.Init(this);
            pickUpButton.Init(this);
            clearButton.Init(this);

            mixButton.SetActive(false);
            pickUpButton.SetActive(false);
            clearButton.SetActive(true);

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                ingradients.Add(ingradientSpawners[i].GetIngradient());
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

                for (int i = 0; i < ingradientCells.Count; i++)
                {
                    ingradientCells[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                }

                pointer.GetComponent<SpriteRenderer>().sortingOrder = ingradientCells.Count + 1;

                StartCoroutine(IColliderCellsEnbled(0f, false));
                StartCoroutine(IColliderSpawnersEnabled(0f, false));
            }

            return pointer;
        }

        public void DragIngradient(IngradientCell ingradientCell)
        {
            this.pointer = ingradientCell;

            Ingradient ingradient = new Ingradient();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientCell.GetTypeIngradient();

            for (int i = 0; i < ingradientCells.Count; i++)
            {
                ingradientCells[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            }

            pointer.GetComponent<SpriteRenderer>().sortingOrder = ingradientCells.Count + 1;

            StartCoroutine(IColliderCellsEnbled(0f, false));
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
            if (isDrag && ingradient != null)
            {
                if(InMixTable())
                {
                    ingradientSpawner.EndDrag();
                    pointer.EndDrag();
                    if(!ingradientCells.Exists(x => x == pointer))
                        ingradientCells.Add(pointer);

                    for (int i = 0; i < ingradientCells.Count; i++)
                    {
                        ingradientCells[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                    }

                    pointer.GetComponent<SpriteRenderer>().sortingOrder = ingradientCells.Count + 1;

                    pointer = null;
                }
                else
                {
                    ingradientSpawner.PutIngradient(countIngradiensTaken);
                    ingradientSpawner.EndDrag();
                    pointer.EndDrag();

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

                if (!ingradientCells.Exists(x => x == pointer))
                    ingradientCells.Add(pointer);

                for (int i = 0; i < ingradientCells.Count; i++)
                {
                    ingradientCells[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                }

                pointer.GetComponent<SpriteRenderer>().sortingOrder = ingradientCells.Count + 1;

                pointer = null;
            }

            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            if (CheckIngradientsInMixTable())
                mixButton.SetActive(true);
            else
                mixButton.SetActive(false);

            StartCoroutine(IColliderSpawnersEnabledWithCondition(0.1f, true));
        }

        public void DropIngradient(IngradientCell ingradientCell)
        {
            Ingradient ingradient = new Ingradient();
            ingradient.countIngradient = countIngradiensTaken;
            ingradient.typeIngradient = ingradientCell.GetTypeIngradient();

            if (isDrag)
            {
                if (InMixTable())
                {
                    ingradientCell.EndDrag();
                    if (!ingradientCells.Exists(x => x == pointer))
                        ingradientCells.Add(ingradientCell);
                }
                else
                {
                    IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetTypeIngradient());
                    ingradientSpawner.PutIngradient(countIngradiensTaken);
                    ingradientCell.EndDrag();

                    ingradientCells.Remove(pointer);

                    Destroy(ingradientCell.gameObject);
                }
            }
            else
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetTypeIngradient());
                ingradientCell.transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                StartCoroutine(IDropIngradient(ingradientCell, ingradientSpawner, timeIngradientMoveToMixTable));
            }

            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            if(CheckIngradientsInMixTable())
                mixButton.SetActive(true);
            else
                mixButton.SetActive(false);

            StartCoroutine(IColliderCellsEnbled(0.1f, true));
            StartCoroutine(IColliderSpawnersEnabledWithCondition(0.1f, true));
        }

        private IEnumerator IColliderCellsEnbled(float time, bool isActive)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < ingradientCells.Count; i++)
            {
                ingradientCells[i].GetComponent<Collider>().enabled = isActive;
            }
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

        private IEnumerator IDropIngradient(IngradientCell ingradientCell, IngradientSpawner ingradientSpawner, float time)
        {
            yield return new WaitForSeconds(time);
            ingradientSpawner.PutIngradient(countIngradiensTaken);

            ingradientCells.Remove(ingradientCell);

            if(ingradientCell != null)
            {
                Destroy(ingradientCell.gameObject);
            }
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

        public void MixIngradients()
        {
            if (CheckIngradientsInMixTable(out List<Ingradient> outIngradients))
            {
                print("Win");
                particleWorkbench.Play();

                for (int j = 0; j < ingradientCells.Count; j++)
                {
                    ingradientCells[j].GetComponent<Collider>().enabled = false;
                    ingradientCells[j].transform.DOMove(mixTable.transform.position, timeNewIngradientCreation).SetEase(Ease.Linear);
                    ingradientCells[j].transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), timeNewIngradientCreation).SetEase(Ease.Linear);
                    ingradientCells[j].transform.DORotate(new Vector3(0, 0, 1440), timeNewIngradientCreation, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                }

                StartCoroutine(IAnimationDeleteIngradients(timeNewIngradientCreation, outIngradients));
                OnMixIngradients?.Invoke();

                mixButton.SetActive(false);
            }
        }

        private IEnumerator IAnimationDeleteIngradients(float time, List<Ingradient> outIngradients)
        {
            yield return new WaitForSeconds(time);

            for (int i = 0; i < ingradientCells.Count; i++)
            {
                Destroy(ingradientCells[i].gameObject);
            }

            ingradientCells.Clear();


            for (int i = 0; i < outIngradients.Count; i++)
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(outIngradients[i].typeIngradient);
                IngradientCell ingradientCell = InstantiatePointer(ingradientSpawner);
                ingradientCell.GetComponent<Collider>().enabled = false;
                ingradientCell.transform.position = new Vector3(mixTable.position.x, ingradientSpawner.transform.position.y, mixTable.position.z);
                ingradientCell.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                ingradientCell.transform.DOScale(prefabDragPoint.transform.localScale, timeNewIngradientCreation).SetEase(Ease.Linear);
                ingradientCell.transform.DORotate(new Vector3(90, 0, 1440), timeNewIngradientCreation, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                ingradientCells.Add(ingradientCell);
            }

            StartCoroutine(IColliderCellsEnbled(time, true));
            StartCoroutine(IAnimationCreateIngradient(time));

        }
        IEnumerator IAnimationCreateIngradient(float time)
        {
            yield return new WaitForSeconds(time);
            particleWorkbench.Stop();         
        }

        public bool CheckIngradientsInMixTable()
        {
            return CheckIngradientsInMixTable(out List<Ingradient> outIngradients);
        }

        public bool CheckIngradientsInMixTable(out List<Ingradient> outIngradients)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                List<Ingradient> ingradientInputCells = new List<Ingradient>();
                List<Ingradient> ingradientInput = new List<Ingradient>();

                for (int j = 0; j < ingradientCells.Count; j++)
                {
                    if (!ingradientInputCells.Exists(x => x.typeIngradient == ingradientCells[j].GetTypeIngradient()))
                    {
                        Ingradient ingradient = new Ingradient();
                        ingradient.countIngradient = countIngradiensTaken;
                        ingradient.typeIngradient = ingradientCells[j].GetTypeIngradient();
                        ingradientInputCells.Add(ingradient);
                    }
                    else
                    {
                        for (int k = 0; k < ingradientInputCells.Count; k++)
                        {
                            if (ingradientInputCells[k].typeIngradient == ingradientCells[j].GetTypeIngradient())
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
                    Debug.Log("Win");
                    return true;
                }
            }

            outIngradients = null;
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
            for (int i = 0; i < ingradientCells.Count; i++)
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCells[i].GetTypeIngradient());
                ingradientCells[i].transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                StartCoroutine(IClearIngradients(ingradientSpawner, ingradientCells[i], timeIngradientMoveToMixTable));
            }

            mixButton.SetActive(false);
        }

        private IEnumerator IClearIngradients(IngradientSpawner ingradientSpawner, IngradientCell ingradientCell, float time)
        {
            yield return new WaitForSeconds(time);
            ingradientCells.Remove(ingradientCell);
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
