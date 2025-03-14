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


            if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems == null)
            {
                SaveManager.filePlayer.JSONPlayer.resources.tableWithItems = new List<CellsData>();

                CellsData cellsData = new CellsData();
                cellsData.nameMasterCells = name;

                List<PickUpItemData> data = new List<PickUpItemData>();

                for (int i = 0; i < cellTables.Count; i++)
                {
                    data.Add(new PickUpItemData());
                }

                cellsData.pickUpItems = data;

                SaveManager.filePlayer.JSONPlayer.resources.tableWithItems.Add(cellsData);
            }
            else
            {
                bool condition = true;
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.tableWithItems.Count; i++)
                {
                    if (SaveManager.filePlayer.JSONPlayer.resources.tableWithItems[i].nameMasterCells == name)
                    {
                        condition = false;
                        break;
                    }
                }

                if (condition)
                {
                    CellsData cellsData = new CellsData();
                    cellsData.nameMasterCells = name;

                    List<PickUpItemData> data = new List<PickUpItemData>();

                    for (int i = 0; i < cellTables.Count; i++)
                    {
                        data.Add(new PickUpItemData());
                    }

                    cellsData.pickUpItems = data;

                    SaveManager.filePlayer.JSONPlayer.resources.tableWithItems.Add(cellsData);
                }

            for (int i = 0; i < cellTables.Count; i++)
            {
                cellTables[i].Init(this, player, i);
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
            }
        }
    }
}
