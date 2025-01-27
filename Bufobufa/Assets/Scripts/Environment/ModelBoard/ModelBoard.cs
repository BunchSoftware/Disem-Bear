using External.DI;
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
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class ModelBoard : MonoBehaviour, IUpdateListener
    {
        [SerializeField] private List<CellModelBoard> cellBoards;
        [SerializeField] private TriggerObject triggerObject;
        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;

        [Header("Drag&Drop")]
        [SerializeField] private Transform draggingParent;
        [SerializeField] private Transform originalParent;
        [SerializeField] private Transform freeDragingParent;

        public UnityEvent OnModelBoardOpen;
        public UnityEvent OnModelBoardClose;

        private SaveManager saveManager;
        private MixTable mixTable;
        private Player player;
        private PlayerMouseMove playerMouseMove;

        public void Init(SaveManager saveManager, MixTable mixTable, Player player, PlayerMouseMove playerMouseMove)
        {
            this.mixTable = mixTable;
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            openObject.OnObjectOpen.AddListener(() =>
            {
                scaleChooseObject.on = false;
                OnModelBoardOpen?.Invoke();
            });
            openObject.OnObjectClose.AddListener(() =>
            {
                scaleChooseObject.on = true;
                OnModelBoardClose?.Invoke();
            });
            openObject.Init(triggerObject, playerMouseMove, player);

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].Init(this, mixTable, player, triggerObject,
                    draggingParent, originalParent, freeDragingParent);
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

        public void OnUpdate(float deltaTime)
        {
            openObject.OnUpdate(deltaTime);
        }

        //public PickUpItem PickUpItem(int indexCell)
        //{
        //    PickUpItem pickUpItem = cellBoards[indexCell].currentItemInCell;

        //    if (cellBoards[indexCell].currentItemInCell != null)
        //    {
        //        OnPickUpItem?.Invoke(cellBoards[indexCell].currentItemInCell);
        //        cellBoards[indexCell].currentItemInCell = null;
        //    }

        //    return pickUpItem;
        //}

        //public bool PutItem(PickUpItem pickUpItem, int indexCell)
        //{
        //    if (cellBoards[indexCell].currentItemInCell == null)
        //    {
        //        OnPutItem?.Invoke(cellBoards[indexCell].currentItemInCell);
        //        cellBoards[indexCell].currentItemInCell.transform.parent = cellBoards[indexCell].cellModelBoard.transform;
        //        cellBoards[indexCell].currentItemInCell.transform.position = cellBoards[indexCell].cellModelBoard.transform.position;

        //        cellBoards[indexCell].currentItemInCell = pickUpItem;

        //        switch (pickUpItem.TypeItem)
        //        {
        //            case TypePickUpItem.None:
        //                break;
        //            case TypePickUpItem.PickUpItem:
        //                break;
        //            case TypePickUpItem.Package:

        //                PackageItem packageItem;
        //                if (player.GetPickUpItem().TryGetComponent(out packageItem))
        //                {
        //                    if (packageItem.ingradients.Count >= 1)
        //                    {
        //                        for (int i = 0; i < packageItem.ingradients.Count; i++)
        //                        {
        //                            mixTable.AddIngradient(packageItem.ingradients[i]);
        //                        }

        //                        PickUpItem item = Instantiate(packageItem.itemInPackage);

        //                        item.transform.parent = cellBoards[indexCell].cellModelBoard.transform;
        //                        item.transform.localPosition = cellBoards[indexCell].cellModelBoard.transform.localPosition;
        //                        cellBoards[indexCell].currentItemInCell = item;

        //                        Destroy(pickUpItem);
        //                    }
        //                }
        //                else
        //                    Debug.LogError("Ошибка. На обьекте нет PackageItem, но обьект указан как Package");

        //                break;
        //        }
        //        return true;
        //    }
        //    return false;
        //}
    }
}
