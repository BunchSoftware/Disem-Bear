using Game.Environment.Item;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Environment.LTableWithItems
{
    [RequireComponent(typeof(ClickableObject))]
    public class CellTableWithItems : MonoBehaviour//, IPointerDownHandler
    {
        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private TableWithItems tableWithItems;
        private TriggerObject triggerObject;

        private PickUpItem currentItemInCell;
        private ClickableObject ItemInCellClickableObject = null;

        private Player player;

        private ClickableObject clickableObject;

        public void Init(TableWithItems tableWithItems, Player player)
        {
            clickableObject = gameObject.GetComponent<ClickableObject>();

            triggerObject = GetComponent<TriggerObject>();

            this.tableWithItems = tableWithItems;
            this.player = player;   

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (clickableObject.MouseClickObject)
                {
                    clickableObject.MouseClickObject = false;

                    if (player.PlayerPickUpItem && PutItem(player.GetPickUpItem()))
                    {
                        player.PutItem();
                        Debug.Log("Я положил предмет в Table");
                    }
                }
                else if (currentItemInCell != null)
                {
                    if (ItemInCellClickableObject.MouseClickObject && player.PlayerPickUpItem == false)
                    {
                        ItemInCellClickableObject.MouseClickObject = false;
                        player.PickUpItem(PickUpItem());
                        Debug.Log("Я поднял предмет из Table");
                    }
                }
            });
        }

        //public void OnPointerDown(PointerEventData eventData)
        //{
        //    if (eventData.button == PointerEventData.InputButton.Left)
        //        isClick = true;
        //    else
        //        isClick = false;
        //}

        public PickUpItem PickUpItem()
        {
            PickUpItem item = null;

            if (currentItemInCell != null)
            {
                ScaleChooseObject scaleChooseObject = currentItemInCell.GetComponent<ScaleChooseObject>();

                if (scaleChooseObject != null)
                    scaleChooseObject.RemoveComponent();

                item = currentItemInCell;

                OnPickUpItem?.Invoke(currentItemInCell);

                gameObject.layer = LayerMask.NameToLayer("ClickedObject");
                currentItemInCell = null;
            }

            return item;
        }

        public bool PutItem(PickUpItem pickUpItem)
        {
            if (currentItemInCell == null)
            {
                OnPutItem?.Invoke(currentItemInCell);

                gameObject.layer = LayerMask.NameToLayer("Default");
                currentItemInCell = pickUpItem;
                currentItemInCell.transform.parent = transform;
                currentItemInCell.transform.position = transform.position;

                if (currentItemInCell.GetComponent<ClickableObject>() == null)
                {
                    ItemInCellClickableObject = currentItemInCell.AddComponent<ClickableObject>();
                }
                else
                {
                    ItemInCellClickableObject = currentItemInCell.GetComponent<ClickableObject>();
                }
                if (currentItemInCell.GetComponent<ScaleChooseObject>() == null)
                {
                    ScaleChooseObject scaleChooseObject = currentItemInCell.AddComponent<ScaleChooseObject>();
                    scaleChooseObject.coefficient = 1.15f;
                }

                return true;
            }

            return false;
        }
    }
}
