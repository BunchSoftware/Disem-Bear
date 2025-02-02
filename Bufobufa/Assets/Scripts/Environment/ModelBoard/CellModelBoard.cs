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
    public class CellModelBoard : MonoBehaviour, ILeftMouseClickable, IMouseOver, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private PickUpItem currentItemInCell;
        private ScaleChooseObject scaleCurrentItemInCell;
        private TriggerObject triggerObject;

        private MixTable mixTable;
        private ModelBoard modelBoard;
        private Player player;

        private bool isClick;

        // Drag&Drop
        private Transform draggingParent;
        private Transform originalParent;
        private Transform freeDragingParent;
        private PickUpItem pointerWithObject;

        public void Init
            (ModelBoard modelBoard, MixTable mixTable, Player player, TriggerObject triggerObject, 
            Transform draggingParent, Transform originalParent, Transform freeDragingParent)
        {
            this.mixTable = mixTable;
            this.triggerObject = triggerObject;
            this.modelBoard = modelBoard;
            this.player = player;

            this.draggingParent = draggingParent;
            this.originalParent = originalParent;
            this.freeDragingParent = freeDragingParent;

            modelBoard.OnModelBoardOpen.AddListener(() =>
            {
                if(scaleCurrentItemInCell != null)
                    scaleCurrentItemInCell.on = true;
            });

            modelBoard.OnModelBoardClose.AddListener(() =>
            {
                if (scaleCurrentItemInCell != null)
                    scaleCurrentItemInCell.on = false;
            });

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (isClick)
                {
                    if(player.PlayerPickUpItem == false)
                        modelBoard.OpenModelBoard();
                    
                    isClick = false;

                    if (player.PlayerPickUpItem && PutItem(player.GetPickUpItem()))
                    {
                        player.PutItem();
                        Debug.Log("Я положил предмет в ModelBoard");
                    }
                }
            });
        }

        public void OnMouseLeftClickObject()
        {
            isClick = true;
        }

        public void OnMouseLeftClickOtherObject()
        {
            isClick = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(pointerWithObject != null)
            {
                pointerWithObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            Debug.Log(1);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentItemInCell != null && pointerWithObject == null)
            {
                pointerWithObject = currentItemInCell;
                pointerWithObject.transform.parent = draggingParent;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //if (In((RectTransform)freeDragingParent, Input.mousePosition))
            //{
            //    InsertInventory();
            //}
            //else
            //{
            //    currentItemInCell = null;
            //    Render(true);
            //    Destroy(pointerWithObject);
            //}
        }

        // На случай. если будем делать механику забирания предмета
        public PickUpItem PickUpItem()
        {
            PickUpItem item = null;

            if (currentItemInCell != null)
            {
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
                OnPutItem?.Invoke(currentItemInCell);

                switch (pickUpItem.TypeItem)
                {
                    case TypePickUpItem.None:
                        break;
                    case TypePickUpItem.PickUpItem:
                        break;
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
                            currentItemInCell = item;

                            if (currentItemInCell.GetComponent<ScaleChooseObject>() == null)
                            {
                                scaleCurrentItemInCell = currentItemInCell.AddComponent<ScaleChooseObject>();
                                scaleCurrentItemInCell.coefficient = 1.08f;
                                scaleCurrentItemInCell.on = false;
                            }

                            Destroy(pickUpItem.gameObject);
                        }
                        else
                            Debug.LogError("Ошибка. На обьекте нет PackageItem, но обьект указан как Package");

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
