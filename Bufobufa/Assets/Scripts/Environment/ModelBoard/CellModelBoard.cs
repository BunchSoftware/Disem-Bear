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

        public void Init(ModelBoard modelBoard, MixTable mixTable, Player player, TriggerObject triggerObject, 
            Transform draggingParent)
        {
            this.mixTable = mixTable;
            this.triggerObject = triggerObject;
            this.modelBoard = modelBoard;
            this.player = player;

            this.draggingParent = draggingParent;

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
            if (currentItemInCell != null)
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

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentItemInCell != null)
            {
                scaleCurrentItemInCell = null;
                currentItemInCell.GetComponent<ScaleChooseObject>().RemoveComponent();
                currentItemInCell.transform.parent = draggingParent;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (InModelBoard())
            {
                modelBoard.InsertPointerObjectToModelBoard(this);
                Debug.Log("OKK");
            }
            else
            {
                Debug.Log("No OKK");
                currentItemInCell.transform.parent = transform;
                currentItemInCell.transform.position = transform.position;
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
                        {
                            pickUpItem.transform.parent = transform;
                            pickUpItem.transform.position = transform.position;
                            currentItemInCell = pickUpItem;

                            Debug.Log(gameObject.name);

                            if (currentItemInCell.GetComponent<ScaleChooseObject>() == null)
                            {
                                scaleCurrentItemInCell = currentItemInCell.AddComponent<ScaleChooseObject>();
                                scaleCurrentItemInCell.coefficient = 1.08f;
                                scaleCurrentItemInCell.on = false;
                            }

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
