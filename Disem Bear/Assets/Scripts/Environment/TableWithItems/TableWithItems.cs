using External.DI;
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

        private Player player;

        public void Init(Player player, GameBootstrap gameBootstrap)
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
                Debug.Log("TableWithItems: Успешно иницилизирован");
            }

            for (int i = 0; i < cellTables.Count; i++)
            {
                cellTables[i].Init(this, player, i, gameBootstrap);
            }
        }
    }
}
