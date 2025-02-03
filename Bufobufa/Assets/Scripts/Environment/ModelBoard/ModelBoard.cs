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
                    draggingParent);
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

        public void OpenModelBoard()
        {
            openObject.OnMouseLeftClickObject();
        }

        public void OnMouseEnterObject()
        {
            scaleChooseObject.OnMouseEnterObject();
        }

        public void OnMouseExitObject()
        {
            scaleChooseObject.OnMouseExitObject();
        }

        public void OnUpdate(float deltaTime)
        {
            openObject.OnUpdate(deltaTime);
        }

        public void InsertPointerObjectToModelBoard(CellModelBoard cellModelBoard)
        {
            PickUpItem curentPickUpItem = cellModelBoard.PickUpItem();
            if (curentPickUpItem != null)
            {
                int closesIndex = 0;

                for (int i = 0; i < cellBoards.Count; i++)
                {
                    if (Vector3.Distance(curentPickUpItem.transform.position, cellBoards[i].transform.position) <=
                        Vector3.Distance(curentPickUpItem.transform.position, cellBoards[closesIndex].transform.position)
                        )
                    {
                        closesIndex = i;
                    }
                }

                Debug.Log("1235");

                Debug.Log(cellBoards[closesIndex].PutItem(curentPickUpItem));
            }
        }
    }
}
