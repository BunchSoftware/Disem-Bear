using DG.Tweening;
using External.DI;
using External.Storage;
using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.Environment.LPostTube;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Game.Environment.LTableWithItems.TableWithItems;

namespace Game.Environment.LModelBoard
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class ModelBoard : MonoBehaviour, IUpdateListener
    {
        [SerializeField] private List<CellModelBoard> cellBoards;
        [SerializeField] private TriggerObject triggerObject;

        [Header("Focus Item")]
        public float coefficientScaleItem = 1.08f;
        public float timeFocusItem = 0.5f;
        public float timeDefocusItem = 0.5f;

        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;

        public UnityEvent OnStartModelBoardOpen;
        public UnityEvent OnEndModelBoardOpen;

        public UnityEvent OnStartModelBoardClose;
        public UnityEvent OnEndModelBoardClose;

        public UnityEvent<PickUpItem> OnDragItem;
        public UnityEvent<PickUpItem> OnDropItem;

        public UnityEvent<PickUpItem> OnFocusItem;
        public UnityEvent<PickUpItem> OnDefocusItem;

        private SaveManager saveManager;
        private MixTable mixTable;
        private Player player;
        private PlayerMouseMove playerMouseMove;

        public bool IsFocus => isFocus;
        private bool isFocus = false;

        public bool IsOpen => isOpen;
        private bool isOpen = false;

        public bool IsDrag => isDrag;
        private bool isDrag = false;

        public void Init(SaveManager saveManager, MixTable mixTable, Player player, PlayerMouseMove playerMouseMove)
        {
            this.mixTable = mixTable;
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            openObject.OnStartObjectOpen.AddListener(() =>
            {
                scaleChooseObject.on = false;
                OnStartModelBoardOpen?.Invoke();
            });
            openObject.OnEndObjectOpen.AddListener(() =>
            {
                isOpen = true;
                OnEndModelBoardOpen?.Invoke();
            });
            openObject.OnStartObjectClose.AddListener(() =>
            {
                OnStartModelBoardClose?.Invoke();
                isOpen = false;
            });
            openObject.OnEndObjectClose.AddListener(() =>
            {
                scaleChooseObject.on = true;
                OnEndModelBoardClose?.Invoke();
            });
            openObject.Init(triggerObject, playerMouseMove, player);

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].Init(this, mixTable, player, triggerObject, transform);
            }

            //if (saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves != null)
            //{
            //    for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves.Count; i++)
            //    {
            //        for (int j = 0; j < getItemFromTables.Count; j++)
            //        {
            //            if (getItemFromTables[j].typeItemFromTable == saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves[i].typeModelBoard)
            //            {
            //                GameObject item = Instantiate(getItemFromTables[j].gameObject.GetComponent<PackageInfo>().ItemInPackage);
            //                items.Add(item);
            //                items[items.Count - 1].transform.parent = transform;
            //                items[items.Count - 1].transform.localPosition = points[items.Count - 1].transform.localPosition;
            //                items[items.Count - 1].SetActive(true);
            //            }
            //        }
            //    }
            //}
        }

        public void FocusItem(CellModelBoard cellModelBoard)
        {
            Vector3 positionCenterScreen = Camera.main.ScreenToWorldPoint(
                new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane + 1)
            );

            openObject.on = false;

            PickUpItem pickUpItem = cellModelBoard.GetCurrentItemInCell();

            pickUpItem.transform.DOMove(new Vector3(positionCenterScreen.x, positionCenterScreen.y, positionCenterScreen.z), timeFocusItem).SetEase(Ease.Linear);
            pickUpItem.transform.DOScale(new Vector3
                (
                pickUpItem.transform.localScale.x * coefficientScaleItem,
                pickUpItem.transform.localScale.y * coefficientScaleItem,
                pickUpItem.transform.localScale.z * coefficientScaleItem
                ), timeFocusItem).SetEase(Ease.Linear);

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].GetComponent<Collider>().enabled = false;
            }

            StartCoroutine(IFocusItem(cellModelBoard, timeFocusItem));
        }

        private IEnumerator IFocusItem(CellModelBoard cellModelBoard, float time)
        {
            yield return new WaitForSeconds(time);

            isFocus = true;
            cellModelBoard.GetCurrentItemInCell().GetComponent<BoxCollider>().enabled = true;
            OnFocusItem?.Invoke(cellModelBoard.GetCurrentItemInCell());
        }

        public void DefocusItem(CellModelBoard cellModelBoard)
        { 
            PickUpItem pickUpItem = cellModelBoard.GetCurrentItemInCell();        
            Vector3 position = pickUpItem.transform.parent.position;

            pickUpItem.transform.DOMove(position, timeFocusItem).SetEase(Ease.Linear);
            pickUpItem.GetComponent<BoxCollider>().enabled = false;

            pickUpItem.transform.DOScale(new Vector3
            (
                pickUpItem.transform.localScale.x / coefficientScaleItem,
                pickUpItem.transform.localScale.y / coefficientScaleItem,
                pickUpItem.transform.localScale.z / coefficientScaleItem
             ), timeFocusItem).SetEase(Ease.Linear);

            StartCoroutine(IDefocusItem(cellModelBoard, timeDefocusItem));
        }

        private IEnumerator IDefocusItem(CellModelBoard cellModelBoard, float time)
        {
            yield return new WaitForSeconds(time);

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].GetComponent<Collider>().enabled = true;
            }

            isFocus = false;
            openObject.on = true;
            OnDefocusItem?.Invoke(cellModelBoard.GetCurrentItemInCell());
        }

        public void OpenModelBoard()
        {
            openObject.OnMouseLeftClickDownObject();
        }

        public void OnMouseEnterObject()
        {
            scaleChooseObject.OnMouseEnterObject();
        }

        public void OnMouseExitObject()
        {
            scaleChooseObject.OnMouseExitObject();
        }

        public void OnUpdate(float deltaTime)
        {
            openObject.OnUpdate(deltaTime);
        }

        public void DragItem(CellModelBoard cellModelBoard)
        {
            if(cellModelBoard.GetCurrentItemInCell())
            {
                isDrag = true;
                OnDragItem?.Invoke(cellModelBoard.GetCurrentItemInCell());

                for (int i = 0; i < cellBoards.Count; i++)
                {
                    cellBoards[i].GetComponent<Collider>().enabled = false;
                }
            }
        }

        private bool InModelBoard()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;

            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxDistance);

            for (int i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].collider.gameObject.TryGetComponent<ModelBoard>(out var modelBoard))
                {
                    return true;
                }
            }

            return false;
        }

        public void DropItem(CellModelBoard cellModelBoard)
        {
            PickUpItem currentPickUpItem = cellModelBoard.PickUpItem();
            if (currentPickUpItem != null && InModelBoard())
            {
                int closesIndex = 0;

                for (int i = 0; i < cellBoards.Count; i++)
                {
                    if (Vector3.Distance(currentPickUpItem.transform.position, cellBoards[i].transform.position) <=
                        Vector3.Distance(currentPickUpItem.transform.position, cellBoards[closesIndex].transform.position)
                        )
                    {
                        closesIndex = i;
                    }
                }

                if (cellBoards[closesIndex].GetCurrentItemInCell() == null)
                {
                    cellBoards[closesIndex].PutItem(currentPickUpItem);
                    cellBoards[closesIndex].isEndDrag = true;
                }
                else
                {
                    PickUpItem exchangePickUpItem = cellBoards[closesIndex].PickUpItem();
                    cellModelBoard.PutItem(exchangePickUpItem);

                    cellBoards[closesIndex].PutItem(currentPickUpItem);
                    cellBoards[closesIndex].isEndDrag = true;
                }
            }
            else if(currentPickUpItem != null)
            {
                currentPickUpItem.transform.parent = transform;
                currentPickUpItem.transform.position = transform.position;
                cellModelBoard.PutItem(currentPickUpItem);
            }

            isDrag = false;
            OnDropItem?.Invoke(currentPickUpItem);

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].GetComponent<Collider>().enabled = true;
            }
        }
    }
}
