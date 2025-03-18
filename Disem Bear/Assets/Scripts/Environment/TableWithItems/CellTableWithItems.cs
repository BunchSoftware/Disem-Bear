using External.DI;
using External.Storage;
using Game.Environment.Item;
using Game.Environment.LModelBoard;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
        private int indexCellTableWithItems = 0;

        public void Init(TableWithItems tableWithItems, Player player, int indexCellModelBoard)
        {
            triggerObject = transform.Find("TriggerObject").GetComponent<TriggerObject>();
            boxCollider = GetComponent<BoxCollider>();

            this.tableWithItems = tableWithItems;
            this.player = player;
            this.indexCellTableWithItems = indexCellModelBoard;

            if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.tableWithItems.Count; i++)
                {
                    if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].nameMasterCells == tableWithItems.name
                        && SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].pickUpItems.Count >= indexCellModelBoard)
                    {
                        PickUpItem condition = GameBootstrap.FindPickUpItemToPrefabs(SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].pickUpItems[indexCellModelBoard].namePickUpItem);

                        if (condition != null)
                        {
                            PickUpItem pickUpItem = Instantiate(condition);

                            pickUpItem.transform.parent = transform;
                            pickUpItem.transform.parent = transform;
                            pickUpItem.transform.position =
                                new Vector3(transform.position.x, transform.position.y + pickUpItem.GetComponent<Collider>().bounds.size.y / 2, transform.position.z);

                            pickUpItem.GetComponent<BoxCollider>().enabled = false;

                            currentItemInCell = pickUpItem;
                        }
                    }
                }
            }

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
                        player.PickUpItem(PickUpItemInCell());
                        Debug.Log("Я поднял предмет из Table");
                    }
                    else
                    {
                        ChangeItems(player.GetPickUpItem(), currentItemInCell);
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

        public void ChangeItems(PickUpItem playerItem, PickUpItem cellItem)
        {
            PickUpItem tempItemInCell = cellItem;
            currentItemInCell = playerItem;
            OnPutItem?.Invoke(currentItemInCell);
            currentItemInCell.transform.parent = transform;
            currentItemInCell.transform.position =
                new Vector3(transform.position.x, transform.position.y + currentItemInCell.GetComponent<Collider>().bounds.size.y / 2, transform.position.z);
            if (currentItemInCell.GetComponent<ScaleChooseObject>() == null)
            {
                ScaleChooseObject scaleChooseObject = currentItemInCell.AddComponent<ScaleChooseObject>();
                scaleChooseObject.coefficient = 1.15f;
            }
            if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.tableWithItems.Count; i++)
                {
                    if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].nameMasterCells == tableWithItems.name
                        && SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].pickUpItems.Count >= indexCellTableWithItems)
                    {
                        SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].pickUpItems[indexCellTableWithItems].namePickUpItem = currentItemInCell.NameItem;
                    }
                }
            }

            SaveManager.UpdatePlayerFile();

            OnPickUpItem?.Invoke(tempItemInCell);
            player.PutItem();
            player.PickUpItem(tempItemInCell);
            Debug.Log("Я поменял объекты местами!");
        }


        public PickUpItem PickUpItemInCell()
        {
            boxCollider.enabled = true;
            PickUpItem item = null;
            item = currentItemInCell;

            OnPickUpItem?.Invoke(currentItemInCell);

            if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.tableWithItems.Count; i++)
                {
                    if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].nameMasterCells == tableWithItems.name
                        && SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].pickUpItems.Count >= indexCellTableWithItems)
                    {
                        SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].pickUpItems[indexCellTableWithItems].namePickUpItem = "";
                    }
                }
            }

            SaveManager.UpdatePlayerFile();

            currentItemInCell = null;

            return item;
        }

        public bool PutItem(PickUpItem pickUpItem)
        {
            if (currentItemInCell == null)
            {
                

                pickUpItem.CanTakeByCollisionPlayer = false;
                boxCollider.enabled = false;
                currentItemInCell = pickUpItem;
                OnPutItem?.Invoke(currentItemInCell);
                currentItemInCell.transform.parent = transform;
                currentItemInCell.transform.position = 
                    new Vector3(transform.position.x, transform.position.y + currentItemInCell.GetComponent<Collider>().bounds.size.y / 2, transform.position.z);

                if (currentItemInCell.GetComponent<ScaleChooseObject>() == null)
                {
                    ScaleChooseObject scaleChooseObject = currentItemInCell.AddComponent<ScaleChooseObject>();
                    scaleChooseObject.coefficient = 1.15f;
                }

                if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems != null)
                {
                    for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.tableWithItems.Count; i++)
                    {
                        if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].nameMasterCells == tableWithItems.name
                            && SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].pickUpItems.Count >= indexCellTableWithItems)
                        {
                            SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].pickUpItems[indexCellTableWithItems].namePickUpItem = pickUpItem.NameItem;
                        }
                    }
                }

                SaveManager.UpdatePlayerFile();

                return true;
            }

            return false;
        }
    }
}
