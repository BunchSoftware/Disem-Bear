using External.DI;
using External.Storage;
using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.Environment.LPostTube;
using Game.Environment.LTableWithItems;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


namespace Game.Environment.LModelBoard
{
    public class CellModelBoard : MonoBehaviour, ILeftMouseUpClickable, IMouseOver, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public UnityEvent<PickUpItem> OnFocusItem;
        public UnityEvent<PickUpItem> OnDefocusItem;
        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private PickUpItem currentItemInCell;
        private ScaleChooseObject scaleChooseObject;
        private TriggerObject triggerObject;
        private GameBootstrap gameBootstrap;
        private AudioClip shh;
        private float timerShh = 10.19f;
        private float timeShh = 10.19f;

        private Workbench workbench;
        private ModelBoard modelBoard;
        private Player player;
        private AudioSource audioSourceShh;

        private bool isClick = false;

        // Drag&Drop
        private Transform draggingParent;
        private int indexCellModelBoard = 0;

        public void Init(ModelBoard modelBoard, Workbench workbench, Player player, TriggerObject triggerObject, Transform draggingParent, int indexCellModelBoard, GameBootstrap gameBootstrap, AudioClip shh)
        {
            this.shh = shh;
            this.gameBootstrap = gameBootstrap;
            this.workbench = workbench;
            this.triggerObject = triggerObject;
            this.modelBoard = modelBoard;
            this.player = player;
            this.indexCellModelBoard = indexCellModelBoard;

            this.draggingParent = draggingParent;

            if (SaveManager.filePlayer.JSONPlayer.resources.modelBoards != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.modelBoards.Count; i++)
                {
                    if (SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].nameMasterCells == modelBoard.name 
                        && SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].pickUpItems.Count >= indexCellModelBoard)
                    {
                        PickUpItem condition = GameBootstrap.FindPickUpItemToPrefabs(SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].pickUpItems[indexCellModelBoard].namePickUpItem);

                        if (condition != null)
                        {
                            PickUpItem pickUpItem = Instantiate(condition, transform);
                            pickUpItem.GetComponent<BoxCollider>().enabled = false;

                            currentItemInCell = pickUpItem;
                            scaleChooseObject = pickUpItem.AddComponent<ScaleChooseObject>();
                        }
                    }
                }
            }

            modelBoard.OnEndModelBoardOpen.AddListener(() =>
            {
                if(currentItemInCell != null && scaleChooseObject != null)
                    scaleChooseObject.on = true;
            });

            modelBoard.OnStartModelBoardClose.AddListener(() =>
            {
                if (currentItemInCell != null && scaleChooseObject != null)
                    scaleChooseObject.on = false;

                if (modelBoard.IsOpen && !modelBoard.IsFocus)
                    modelBoard.DropItem(this);
            });

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                
                if (isClick && !modelBoard.IsOpen)
                {

                    if (player.PlayerPickUpItem == false)
                        modelBoard.OpenModelBoard();

                    isClick = false;

                    if (player.PlayerPickUpItem && PutItem(player.GetPickUpItem()))
                    {
                        player.PutItem();
                        Debug.Log($"Предмет {currentItemInCell.NameItem} был помещен в ячейку {name} ModelBoard");
                    }
                }
                else if (!modelBoard.IsDrag && isClick && modelBoard.IsOpen &&
                        currentItemInCell != null && !currentItemInCell.IsClickedRightMouseButton)
                {
                    isClick = false;
                    modelBoard.FocusItem(this);
                    OnFocusItem?.Invoke(currentItemInCell);
                }
                else if (!modelBoard.IsDrag && modelBoard.IsOpen && currentItemInCell != null && currentItemInCell.IsClickedRightMouseButton)
                {
                    currentItemInCell.IsClickedRightMouseButton = false;
                    modelBoard.DefocusItem(this);
                    OnDefocusItem?.Invoke(currentItemInCell);
                }
            });
        }

        public void OnUpdate(float deltaTime)
        {
            if (timerShh < timeShh)
            {
                timerShh += deltaTime;
            }
        }

        public void OnMouseLeftClickUpObject()
        {
             if(!modelBoard.IsEndDrag)
                isClick = true;
        }

        public void OnMouseLeftClickUpOtherObject()
        {
            isClick = false;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && currentItemInCell != null && modelBoard.IsOpen && !modelBoard.IsFocus)
            {
                modelBoard.DragItem(this);
                audioSourceShh = gameBootstrap.OnPlayOneShotSound(shh);
                timerShh = 0;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && currentItemInCell != null && modelBoard.IsOpen && !modelBoard.IsFocus)
            {
                if (timerShh >= timeShh)
                {
                    audioSourceShh = gameBootstrap.OnPlayOneShotSound(shh);
                    timerShh = 0;
                }
                Vector3 position = ScreenPositionInWorldPosition.GetWorldPositionOnPlaneYZ(Input.mousePosition,  modelBoard.transform.position.x);

                currentItemInCell.transform.position =
                    new Vector3(
                        currentItemInCell.transform.position.x,
                        position.y,
                        position.z
                    );
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            gameBootstrap.OnEndPlayOneSHotSound(audioSourceShh);
            if (modelBoard.IsOpen && !modelBoard.IsFocus)
                modelBoard.DropItem(this);
        }

        public PickUpItem GetCurrentItemInCell()
        {
            return currentItemInCell;
        }

        public PickUpItem PickUpItem()
        {
            PickUpItem item = null;

            if (currentItemInCell != null)
            {
                currentItemInCell.GetComponent<BoxCollider>().enabled = true;
                item = currentItemInCell;

                OnPickUpItem?.Invoke(currentItemInCell);

                if (SaveManager.filePlayer.JSONPlayer.resources.modelBoards != null)
                {
                    for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.modelBoards.Count; i++)
                    {
                        if (SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].nameMasterCells == modelBoard.name
                            && SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].pickUpItems.Count >= indexCellModelBoard)
                        {
                            SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].pickUpItems[indexCellModelBoard].namePickUpItem = "";
                        }
                    }
                }

                SaveManager.UpdatePlayerFile();

                currentItemInCell = null;
                scaleChooseObject = null;
            }

            return item;
        }

        public bool PutItem(PickUpItem pickUpItem)
        {
            if (currentItemInCell == null && pickUpItem != null)
            {
                switch (pickUpItem.TypeItem)
                {
                    case TypePickUpItem.None:
                        break;
                    case TypePickUpItem.ModelBoardItem:
                        {
                            pickUpItem.transform.parent = transform;
                            pickUpItem.transform.position = transform.position;
                            pickUpItem.GetComponent<BoxCollider>().enabled = false;

                            if (SaveManager.filePlayer.JSONPlayer.resources.modelBoards != null)
                            {
                                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.modelBoards.Count; i++)
                                {
                                    if (SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].nameMasterCells == modelBoard.name
                                        && SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].pickUpItems.Count >= indexCellModelBoard)
                                    {
                                        SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].pickUpItems[indexCellModelBoard].namePickUpItem = pickUpItem.NameItem;
                                    }
                                }
                            }

                            currentItemInCell = pickUpItem;
                            OnPutItem?.Invoke(currentItemInCell);

                            if(pickUpItem.GetComponent<ScaleChooseObject>() == null)
                                scaleChooseObject = pickUpItem.AddComponent<ScaleChooseObject>();

                            SaveManager.UpdatePlayerFile();

                            return true;
                        }
                    case TypePickUpItem.Package:

                        PackageItem packageItem;
                        if (player.GetPickUpItem().TryGetComponent(out packageItem))
                        {
                            if (packageItem.ingradients.Count >= 1)
                            {
                                for (int i = 0; i < packageItem.ingradients.Count; i++)
                                {
                                    workbench.AddIngradient(packageItem.ingradients[i]);
                                }
                            }

                            PickUpItem item = Instantiate(packageItem.itemInPackage, transform);

                            if (SaveManager.filePlayer.JSONPlayer.resources.modelBoards != null)
                            {
                                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.modelBoards.Count; i++)
                                {
                                    if (SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].nameMasterCells == modelBoard.name
                                        && SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].pickUpItems.Count >= indexCellModelBoard)
                                    {
                                        SaveManager.filePlayer.JSONPlayer.resources.modelBoards[i].pickUpItems[indexCellModelBoard].namePickUpItem = item.NameItem;
                                    }
                                }
                            }

                            item.transform.parent = transform;
                            item.transform.position = transform.position;
                            pickUpItem.GetComponent<BoxCollider>().enabled = false;
                            currentItemInCell = item;

                            if (item.GetComponent<ScaleChooseObject>() == null)
                                scaleChooseObject = item.AddComponent<ScaleChooseObject>();

                            SaveManager.UpdatePlayerFile();

                            Destroy(pickUpItem.gameObject);
                        }
                        else
                            Debug.LogError("Ошибка. На обьекте нет PackageItem, но обьект указан как Package");

                        OnPutItem?.Invoke(currentItemInCell);


                        return true;
                }
            }

            return false;
        }

        public void OnMouseEnterObject()
        {
            modelBoard.OnMouseEnterObject();
        }

        public void OnMouseExitObject()
        {
            modelBoard.OnMouseExitObject();
        }
    }
}
