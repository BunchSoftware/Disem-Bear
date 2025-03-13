using External.Storage;
using Game.Environment.Item;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.LTableWithItems
{
    public class TableWithItems : MonoBehaviour
    {
        [SerializeField] private List<CellTableWithItems> cellTables = new List<CellTableWithItems>();
        [Tooltip("Список предметов, которые будут на полке в начале")]
        [SerializeField] private List<PickUpItem> itemsPutInStart = new(); 

        private Player player;

        public void Init(Player player)
        {
            this.player = player;

            for (int i = 0; i < cellTables.Count; i++)
            {
                cellTables[i].Init(this, player);
                if (i < itemsPutInStart.Count)
                {
                    if (PrefabUtility.IsPartOfPrefabAsset(itemsPutInStart[i]))
                    {
                        itemsPutInStart[i] = Instantiate(itemsPutInStart[i]);
                    }
                    cellTables[i].PutItem(itemsPutInStart[i]);
                }
            }
            itemsPutInStart.Clear();

            

            Debug.Log("TableWithItems: Успешно иницилизирован");

            //triggerObject.OnTriggerEnterEvent.AddListener((collider) =>
            //{
            //    isTrigger = true;
            //});

            //triggerObject.OnTriggerExitEvent.AddListener((collider) =>
            //{
            //    isTrigger = false;
            //});

            //if (SaveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves != null)
            //{
            //    for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Count; i++)
            //    {
            //        for (int j = 0; j < getItemFromTables.Count; j++)
            //        {
            //            if (SaveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].typeItemFromTable == getItemFromTables[j].typeItemFromTable)
            //            {
            //                GetItemFromTable getItemFromTable = Instantiate(getItemFromTables[j]);
            //                getItemFromTable.indexPoint = SaveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].indexPoint;
            //            }
            //        }
            //    }
            //    if (SaveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave != null
            //        && SaveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave.typeItemFromTable != "")
            //    {
            //        for (int j = 0; j < getItemFromTables.Count; j++)
            //        {
            //            if (SaveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave.typeItemFromTable == getItemFromTables[j].typeItemFromTable)
            //            {
            //                GetItemFromTable getItemFromTable = Instantiate(getItemFromTables[j]);
            //                getItemFromTable.indexPoint = SaveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave.indexPoint;
            //            }
            //        }
            //    }
            //}
        }

        //public bool TakeObject(ItemFromTableSave itemFromTableSave)
        //{
        //    for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Count; i++)
        //    {
        //        if (SaveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].indexPoint == itemFromTableSave.indexPoint)
        //        {
        //            SaveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.RemoveAt(i);
        //            SaveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave = new ItemFromTableSave() 
        //            { 
        //                typeItemFromTable = itemFromTableSave.typeItemFromTable,
        //                indexPoint = itemFromTableSave.indexPoint,
        //            };
        //            SaveManager.UpdatePlayerFile();
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
