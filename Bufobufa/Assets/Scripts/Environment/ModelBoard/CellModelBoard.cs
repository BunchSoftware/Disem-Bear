using External.Storage;
using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.Environment.LPostTube;
using Game.Environment.LTableWithItems;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Game.Environment.LModelBoard
{
    [RequireComponent(typeof(ScaleChooseObject))]
    public class CellModelBoard : MonoBehaviour, ILeftMouseUpClickable, IMouseOver, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public UnityEvent<PickUpItem> OnFocusItem;
        public UnityEvent<PickUpItem> OnDefocusItem;
        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private PickUpItem currentItemInCell;
        private ScaleChooseObject scaleChooseObject;
        private TriggerObject triggerObject;

        private MixTable mixTable;
        private ModelBoard modelBoard;
        private Player player;

        private bool isClick;

        [HideInInspector] public bool isEndDrag = false;

        // Drag&Drop
        private Transform draggingParent;

        public void Init(ModelBoard modelBoard, MixTable mixTable, Player player, TriggerObject triggerObject, 
            Transform draggingParent)
        {
            this.mixTable = mixTable;
            this.triggerObject = triggerObject;
            this.modelBoard = modelBoard;
            this.player = player;

            this.draggingParent = draggingParent;

            scaleChooseObject = GetComponent<ScaleChooseObject>();

            modelBoard.OnEndModelBoardOpen.AddListener(() =>
            {
                if(currentItemInCell != null && scaleChooseObject != null)
                    scaleChooseObject.on = true;
            });

            modelBoard.OnStartModelBoardClose.AddListener(() =>
            {
                if (currentItemInCell != null && scaleChooseObject != null)
                    scaleChooseObject.on = false;
            });

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (isClick && !modelBoard.IsOpen)
                {
                    if (player.PlayerPickUpItem == false)
                        modelBoard.OpenModelBoard();

                    isClick = false;

                    if (player.PlayerPickUpItem && PutItem(player.GetPickUpItem()))
                    {
                        player.PutItem();
                        Debug.Log("Я положил предмет в ModelBoard");
                    }
                }
                else if (!modelBoard.IsDrag && isClick && modelBoard.IsOpen &&
                        currentItemInCell != null && !currentItemInCell.IsClickedRightMouseButton)
                {
                    isClick = false;
                    modelBoard.FocusItem(this);
                    OnFocusItem?.Invoke(currentItemInCell);
                }
                else if (!modelBoard.IsDrag && modelBoard.IsOpen && currentItemInCell != null && currentItemInCell.IsClickedRightMouseButton)
                {
                    currentItemInCell.IsClickedRightMouseButton = false;
                    modelBoard.DefocusItem(this);
                    OnDefocusItem?.Invoke(currentItemInCell);
                }
            });
        }

        public void OnMouseLeftClickUpObject()
        {
            if(isEndDrag)
                isEndDrag = false;
            else
                isClick = true;
        }

        public void OnMouseLeftClickUpOtherObject()
        {
            isClick = false;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentItemInCell != null && modelBoard.IsOpen && !modelBoard.IsFocus)
                modelBoard.DragItem(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (currentItemInCell != null && modelBoard.IsOpen && !modelBoard.IsFocus)
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 1)
                );

                currentItemInCell.transform.position =
                    new Vector3(
                        currentItemInCell.transform.position.x,
                        position.y,
                        position.z
                    );
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!modelBoard.IsFocus)
            {
                if (InModelBoard() && modelBoard.IsOpen)
                {
                    modelBoard.DropItem(this);
                }
                else if (currentItemInCell != null)
                {
                    currentItemInCell.transform.parent = transform;
                    currentItemInCell.transform.position = transform.position;
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

        public PickUpItem GetCurrentItemInCell()
        {
            return currentItemInCell;
        }

        public PickUpItem PickUpItem()
        {
            PickUpItem item = null;

            if (currentItemInCell != null)
            {
                currentItemInCell.GetComponent<BoxCollider>().enabled = true;
                item = currentItemInCell;

                OnPickUpItem?.Invoke(currentItemInCell);
                currentItemInCell = null;
            }

            return item;
        }

        public bool PutItem(PickUpItem pickUpItem)
        {
            if (currentItemInCell == null)
            {
                switch (pickUpItem.TypeItem)
                {
                    case TypePickUpItem.None:
                        break;
                    case TypePickUpItem.PickUpItem:
                        {
                            pickUpItem.transform.parent = transform;
                            pickUpItem.transform.position = transform.position;
                            pickUpItem.GetComponent<BoxCollider>().enabled = false;

                            currentItemInCell = pickUpItem;
                            OnPutItem?.Invoke(currentItemInCell);

                            break;
                        }
                    case TypePickUpItem.Package:

                        PackageItem packageItem;
                        if (player.GetPickUpItem().TryGetComponent(out packageItem))
                        {
                            if (packageItem.ingradients.Count >= 1)
                            {
                                for (int i = 0; i < packageItem.ingradients.Count; i++)
                                {
                                    mixTable.AddIngradient(packageItem.ingradients[i]);
                                }
                            }

                            PickUpItem item = Instantiate(packageItem.itemInPackage, transform);

                            item.transform.parent = transform;
                            item.transform.position = transform.position;
                            pickUpItem.GetComponent<BoxCollider>().enabled = false;
                            currentItemInCell = item;

                            Destroy(pickUpItem.gameObject);
                        }
                        else
                            Debug.LogError("Ошибка. На обьекте нет PackageItem, но обьект указан как Package");

                        OnPutItem?.Invoke(currentItemInCell);

                        break;
                }

                return true;
            }

            return false;
        }

        public void OnMouseEnterObject()
        {
            modelBoard.OnMouseEnterObject();
        }

        public void OnMouseExitObject()
        {
            modelBoard.OnMouseExitObject();
        }
    }
}
