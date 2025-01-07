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
    public class ModelBoard : MonoBehaviour, IPickUpItem, IPutItem
    {
        [Serializable]
        class CellBoard
        {
            public CellModelBoard cellModelBoard;
            public PickUpItem currentItemInCell;
        }
        [SerializeField] private List<CellBoard> cellBoards;
        [SerializeField] private TriggerObject triggerObject;

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private SaveManager saveManager;
        private MixTable mixTable;
        private Player player;

        public void Init(SaveManager saveManager, MixTable mixTable, Player player)
        {
            this.mixTable = mixTable;
            this.saveManager = saveManager;
            this.player = player;

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].cellModelBoard.Init(this, player);
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

        public PickUpItem PickUpItem(int indexCell)
        {
            PickUpItem pickUpItem = cellBoards[indexCell].currentItemInCell;

            if (cellBoards[indexCell].currentItemInCell != null)
            {
                OnPickUpItem?.Invoke(cellBoards[indexCell].currentItemInCell);
                cellBoards[indexCell].currentItemInCell = null;
            }

            return pickUpItem;
        }

        public bool PutItem(PickUpItem pickUpItem, int indexCell)
        {
            if (cellBoards[indexCell].currentItemInCell == null)
            {
                OnPutItem?.Invoke(cellBoards[indexCell].currentItemInCell);
                cellBoards[indexCell].currentItemInCell.transform.parent = cellBoards[indexCell].cellModelBoard.transform;
                cellBoards[indexCell].currentItemInCell.transform.position = cellBoards[indexCell].cellModelBoard.transform.position;

                cellBoards[indexCell].currentItemInCell = pickUpItem;

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

                                PickUpItem item = Instantiate(packageItem.itemInPackage);

                                item.transform.parent = cellBoards[indexCell].cellModelBoard.transform;
                                item.transform.localPosition = cellBoards[indexCell].cellModelBoard.transform.localPosition;
                                cellBoards[indexCell].currentItemInCell = item;

                                Destroy(pickUpItem);
                            }
                        }
                        else
                            Debug.LogError("Ошибка. На обьекте нет PackageItem, но обьект указан как Package");

                        break;
                }
                return true;
            }
            return false;
        }
    }
}
