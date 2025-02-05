using DG.Tweening;
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
using static UnityEditor.Progress;

namespace Game.Environment.LModelBoard
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class ModelBoard : MonoBehaviour, IUpdateListener
    {
        [SerializeField] private List<CellModelBoard> cellBoards;
        [SerializeField] private TriggerObject triggerObject;
        public float timeFocusItem = 1f;
        public float timeDefocusItem = 1f;

        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;

        public UnityEvent OnStartModelBoardOpen;
        public UnityEvent OnEndModelBoardOpen;

        public UnityEvent OnStartModelBoardClose;
        public UnityEvent OnEndModelBoardClose;

        public UnityEvent<PickUpItem> OnFocusItem;
        public UnityEvent<PickUpItem> OnDefocusItem;

        private SaveManager saveManager;
        private MixTable mixTable;
        private Player player;
        private PlayerMouseMove playerMouseMove;

        public bool IsOpen => isOpen;
        private bool isOpen = false;

        public void Init(SaveManager saveManager, MixTable mixTable, Player player, PlayerMouseMove playerMouseMove)
        {
            this.mixTable = mixTable;
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            openObject.OnStartObjectOpen.AddListener(() =>
            {
                scaleChooseObject.on = false;
                OnStartModelBoardOpen?.Invoke();
            });
            openObject.OnEndObjectOpen.AddListener(() =>
            {
                isOpen = true;
                OnEndModelBoardOpen?.Invoke();
            });
            openObject.OnStartObjectClose.AddListener(() =>
            {
                OnStartModelBoardClose?.Invoke();
            });
            openObject.OnEndObjectClose.AddListener(() =>
            {
                scaleChooseObject.on = true;
                isOpen = false;
                OnEndModelBoardClose?.Invoke();
            });
            openObject.Init(triggerObject, playerMouseMove, player);

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].Init(this, mixTable, player, triggerObject, transform);
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

        public void FocusItem(PickUpItem item)
        {
            Vector3 positionCenterScreen = Camera.main.ScreenToWorldPoint(
                new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane + 1)
            );

            openObject.on = false;
            item.transform.DOMove(new Vector3(positionCenterScreen.x, positionCenterScreen.y, positionCenterScreen.z), timeFocusItem).SetEase(Ease.Linear);

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].GetComponent<Collider>().enabled = false;
            }

            StartCoroutine(IFocusItem(item, timeFocusItem));
        }

        private IEnumerator IFocusItem(PickUpItem item, float time)
        {
            yield return new WaitForSeconds(time);

            item.GetComponent<BoxCollider>().enabled = true;
            OnFocusItem?.Invoke(item);
        }

        public void DefocusItem(PickUpItem item)
        {
            Vector3 position = item.transform.parent.position;

            item.transform.DOMove(position, timeFocusItem).SetEase(Ease.Linear);
            item.GetComponent<BoxCollider>().enabled = false;

            StartCoroutine(IDefocusItem(item, timeDefocusItem));
        }

        private IEnumerator IDefocusItem(PickUpItem item, float time)
        {
            yield return new WaitForSeconds(time);

            for (int i = 0; i < cellBoards.Count; i++)
            {
                cellBoards[i].GetComponent<Collider>().enabled = true;
            }

            openObject.on = true;
            OnDefocusItem?.Invoke(item);
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


                cellBoards[closesIndex].PutItem(curentPickUpItem);
            }
        }
    }
}
