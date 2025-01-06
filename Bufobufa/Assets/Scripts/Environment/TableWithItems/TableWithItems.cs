using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.TableWithItems
{
    public class TableWithItems : MonoBehaviour, IPickUpItem, IPutItem
    {
        [System.Serializable]
        public class CellTable
        {
            public CellTableWithItems cellTableWithItems;
            [HideInInspector] public PickUpItem currentItemInCell;
        }

        [SerializeField] private List<CellTable> cellTables = new List<CellTable>();

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private SaveManager saveManager;

        public void Init(SaveManager saveManager)
        {
            this.saveManager = saveManager;

            for (int i = 0; i < cellTables.Count; i++)
            {
                cellTables[i].cellTableWithItems.Init(this);
            }

            //if (saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves != null)
            //{
            //    for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Count; i++)
            //    {
            //        for (int j = 0; j < getItemFromTables.Count; j++)
            //        {
            //            if (saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].typeItemFromTable == getItemFromTables[j].typeItemFromTable)
            //            {
            //                GetItemFromTable getItemFromTable = Instantiate(getItemFromTables[j]);
            //                getItemFromTable.indexPoint = saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].indexPoint;
            //            }
            //        }
            //    }
            //    if (saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave != null
            //        && saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave.typeItemFromTable != "")
            //    {
            //        for (int j = 0; j < getItemFromTables.Count; j++)
            //        {
            //            if (saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave.typeItemFromTable == getItemFromTables[j].typeItemFromTable)
            //            {
            //                GetItemFromTable getItemFromTable = Instantiate(getItemFromTables[j]);
            //                getItemFromTable.indexPoint = saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave.indexPoint;
            //            }
            //        }
            //    }
            //}
        }

        public PickUpItem PickUpItem(int indexCell)
        {
            PickUpItem pickUpItem = cellTables[indexCell].currentItemInCell;

            if (cellTables[indexCell].currentItemInCell != null)
            {
                OnPickUpItem?.Invoke(cellTables[indexCell].currentItemInCell);
                cellTables[indexCell].currentItemInCell = null;
            }

            return pickUpItem;
        }

        public bool PutItem(PickUpItem pickUpItem, int indexCell)
        {
            if (cellTables[indexCell].currentItemInCell == null)
            {
                OnPutItem?.Invoke(cellTables[indexCell].currentItemInCell);
                cellTables[indexCell].currentItemInCell.transform.parent = cellTables[indexCell].cellTableWithItems.transform;
                cellTables[indexCell].currentItemInCell.transform.position = cellTables[indexCell].cellTableWithItems.transform.position;

                cellTables[indexCell].currentItemInCell = pickUpItem;
                return true;
            }

            return false;
        }

        //public bool TakeObject(ItemFromTableSave itemFromTableSave)
        //{
        //    for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Count; i++)
        //    {
        //        if (saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].indexPoint == itemFromTableSave.indexPoint)
        //        {
        //            saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.RemoveAt(i);
        //            saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave = new ItemFromTableSave() 
        //            { 
        //                typeItemFromTable = itemFromTableSave.typeItemFromTable,
        //                indexPoint = itemFromTableSave.indexPoint,
        //            };
        //            saveManager.UpdatePlayerFile();
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
