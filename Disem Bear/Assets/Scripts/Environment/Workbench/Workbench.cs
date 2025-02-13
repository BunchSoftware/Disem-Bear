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
            clearButton.Init(this);

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

            for (int i = 0; i < ingradientCells.Count; i++)
            {
                ingradientCells[i].GetComponent<Collider>().enabled = false;
            }

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                ingradientSpawners[i].GetComponent<Collider>().enabled = false;
            }

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
                    if (!ingradientCells.Exists(x => x == pointer))
                        ingradientCells.Add(pointer);
                    pointer = null;
                }
                else
                {
                    IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetTypeIngradient());
                    ingradientSpawner.PutIngradient(countIngradiensTaken);
                    ingradientCell.EndDrag();

                    ingradientCells.Remove(pointer);

                    Destroy(pointer.gameObject);

                    pointer = null;
                }
            }
            else
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCell.GetTypeIngradient());
                pointer.transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                StartCoroutine(IDropIngradient(ingradientSpawner, timeIngradientMoveToMixTable));
            }

            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            StartCoroutine(IColliderEnbled(0.1f));

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                if (ingradientSpawners[i].GetIngradient().countIngradient > 0)
                    ingradientSpawners[i].GetComponent<Collider>().enabled = true;
            }
        }

        private IEnumerator IColliderEnbled(float time)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < ingradientCells.Count; i++)
            {
                ingradientCells[i].GetComponent<Collider>().enabled = true;
            }
        }

        private IEnumerator IDropIngradient(IngradientSpawner ingradientSpawner, float time)
        {
            yield return new WaitForSeconds(time);
            ingradientSpawner.PutIngradient(countIngradiensTaken);

            ingradientCells.Remove(pointer);

            if(pointer != null)
            {
                Destroy(pointer.gameObject);
                pointer = null;
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
                        ingradient.countIngradient = ingradientInput.Count;
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

                ingradientInputCells.OrderBy(x => x.typeIngradient);
                ingradientInput.OrderBy(x => x.typeIngradient);

                print(ingradientInputCells.Count);
                print(ingradientInput.Count);

                if(CheckIngradients(ingradientInput, ingradientInputCells))
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

                    StartCoroutine(IAnimationDeleteIngradients(timeNewIngradientCreation, recipes[i].outIngradients));
                    OnMixIngradients?.Invoke();
                }
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

            StartCoroutine(IColliderEnbled(time));
            StartCoroutine(IAnimationCreateIngradient(time));

        }
        IEnumerator IAnimationCreateIngradient(float time)
        {
            yield return new WaitForSeconds(time);
            particleWorkbench.Stop();         
        }

        private bool CheckIngradients(List<Ingradient> ingradientInput, List<Ingradient> ingradientInputCells)
        {
            if (ingradientInput.Count == ingradientInputCells.Count)
            {
                for (int j = 0; j < ingradientInput.Count; j++)
                {
                    if (ingradientInput[j].typeIngradient != ingradientInputCells[j].typeIngradient
                        && ingradientInputCells[j].countIngradient != ingradientInput[j].countIngradient)
                        return false;
                }
            }
            else
                return false;

            return true;
        }

        public void ClearIngredients()
        {
            for (int i = 0; i < ingradientCells.Count; i++)
            {
                IngradientSpawner ingradientSpawner = GetIngradientSpawnerOfTypeIngradient(ingradientCells[i].GetTypeIngradient());
                ingradientCells[i].transform.DOMove(ingradientSpawner.transform.position, timeIngradientMoveToMixTable).SetEase(Ease.Linear);
                StartCoroutine(IClearIngradients(ingradientSpawner, ingradientCells[i], timeIngradientMoveToMixTable));
            }
            OnClearIngradients?.Invoke();
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
    }
}
