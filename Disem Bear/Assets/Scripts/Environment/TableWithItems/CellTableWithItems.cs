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
    public class CellTableWithItems : MonoBehaviour, ILeftMouseDownClickable
    {
        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private TableWithItems tableWithItems;
        private TriggerObject triggerObject;
        private BoxCollider boxCollider;

        private PickUpItem currentItemInCell;

        private Player player;

        private bool isClick = false;

        public void Init(TableWithItems tableWithItems, Player player)
        {
            triggerObject = transform.Find("TriggerObject").GetComponent<TriggerObject>();
            boxCollider = GetComponent<BoxCollider>();

            this.tableWithItems = tableWithItems;
            this.player = player;

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (isClick)
                {
                    isClick = false;
                    if (player.PlayerPickUpItem && PutItem(player.GetPickUpItem()))
                    {
                        player.PutItem();
                        Debug.Log("Я положил предмет в Table");
                    }
                }
                else if (currentItemInCell != null && currentItemInCell.IsClickedLeftMouseButton)
                {
                    currentItemInCell.IsClickedLeftMouseButton = false;

                    if (player.PlayerPickUpItem == false)
                    {
                        player.PickUpItem(PickUpItem());
                        Debug.Log("Я поднял предмет из Table");
                    }
                }
            });
        }

        public void OnMouseLeftClickDownObject()
        {
            isClick = true;
        }

        public void OnMouseLeftClickDownOtherObject()
        {
            isClick = false;
        }


        public PickUpItem PickUpItem()
        {
            boxCollider.enabled = true;
            PickUpItem item = null;
            item = currentItemInCell;

            OnPickUpItem?.Invoke(currentItemInCell);

            currentItemInCell = null;

            return item;
        }

        public bool PutItem(PickUpItem pickUpItem)
        {
            if (currentItemInCell == null)
            {
                OnPutItem?.Invoke(currentItemInCell);

                pickUpItem.CanTakeByCollisionPlayer = false;
                boxCollider.enabled = false;
                currentItemInCell = pickUpItem;
                currentItemInCell.transform.parent = transform;
                currentItemInCell.transform.position = 
                    new Vector3(transform.position.x, transform.position.y + currentItemInCell.GetComponent<Collider>().bounds.size.y / 2, transform.position.z);

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
