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
    public class CellTableWithItems : MonoBehaviour, IPointerDownHandler
    {
        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private TableWithItems tableWithItems;
        private TriggerObject triggerObject;

        private PickUpItem currentItemInCell;

        private Player player;
        private bool isClick;

        public void Init(TableWithItems tableWithItems, Player player)
        {
            triggerObject = GetComponent<TriggerObject>();

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
                    else if (player.PlayerPickUpItem == false)
                    {
                        player.PickUpItem(PickUpItem());
                        Debug.Log("Я поднял предмет из Table");
                    }
                }
            });
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                isClick = true;
            else
                isClick = false;
        }

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
                currentItemInCell = null;
            }

            return item;
        }

        public bool PutItem(PickUpItem pickUpItem)
        {
            if (currentItemInCell == null)
            {
                OnPutItem?.Invoke(currentItemInCell);

                currentItemInCell = pickUpItem;
                currentItemInCell.transform.parent = transform;
                currentItemInCell.transform.position = transform.position;

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
