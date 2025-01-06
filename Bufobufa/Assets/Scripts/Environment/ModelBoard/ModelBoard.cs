using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.ModelBoard
{
    public class ModelBoard : MonoBehaviour, IPickUpItem, IPutItem
    {
        [Serializable]
        class CellModelBoard
        {
            public CellModelBoard cellModelBoard;
            public PickUpItem currentItemInCell;
        }
        [SerializeField] private List<CellModelBoard> cellModelBoards;
        [SerializeField] private TriggerObject triggerObject;

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private SaveManager saveManager;
        private MixTable mixTable;

        public void Init(SaveManager saveManager)
        {
            this.saveManager = saveManager;

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
        private void Update()
        {
            if (GetComponent<OpenObject>().ObjectIsOpen && Workbench.GetComponent<OpenObject>().ObjectIsOpen && OneTap)
            {
                OneTap = false;
                Workbench.GetComponent<OpenObject>().ArgumentsNotQuit += 1;
                GetComponent<OpenObject>().ArgumentsNotQuit += 1;
                InTableAndBoard = true;
            }
            if (GetComponent<OpenObject>().ArgumentsNotQuit != 2 && InTableAndBoard && !GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().ObjectIsOpen && Input.GetMouseButtonDown(1))
            {
                OneTap = true;
                //GetComponent<OpenObject>().TriggerObject.SetActive(false);
                GetComponent<OpenObject>().ObjectIsOpen = false;
                GetComponent<OpenObject>().ObjectAnim = true;
                GetComponent<OpenObject>().ClickedMouse = false;

                GetComponent<OpenObject>().Vcam.GetComponent<MoveCameraAnimation>().EndMove();

                StartCoroutine(WaitAnimTable(GetComponent<OpenObject>().Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation + 0.1f));
                StartCoroutine(WaitAnimCamera(GetComponent<OpenObject>().Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation + 0.1f));

                GetComponent<BoxCollider>().enabled = true;
                GetComponent<OpenObject>().ArgumentsNotQuit -= 1;
                InTableAndBoard = false;
            }

            if (!GetComponent<OpenObject>().ObjectIsOpen && GetComponent<OpenObject>().InTrigger && GetComponent<OpenObject>().ClickedMouse && Player.GetComponent<Player>().PlayerPickUpItem)
            {
                GetComponent<OpenObject>().ClickedMouse = false;
                if (Player.GetComponent<Player>().currentPickObject.GetComponent<PackageInfo>())
                {
                    if (Player.GetComponent<Player>().currentPickObject.GetComponent<PackageInfo>().PackageName == "Document")
                    {
                        if (items.Count < points.Count)
                        {
                            if (Player.GetComponent<Player>().currentPickObject.GetComponent<PackageInfo>().HaveIngredients)
                            {
                                for (int i = 0; i < Player.GetComponent<Player>().currentPickObject.GetComponent<PackageInfo>().amount; i++)
                                {
                                    mixTable.AddIngridient(Player.GetComponent<Player>().currentPickObject.GetComponent<PackageInfo>().NameIngredient);
                                }
                            }


                            GameObject item = Instantiate(Player.GetComponent<Player>().currentPickObject.GetComponent<PackageInfo>().ItemInPackage);
                            items.Add(item);
                            items[items.Count - 1].transform.parent = transform;
                            items[items.Count - 1].transform.localPosition = points[items.Count - 1].transform.localPosition;
                            items[items.Count - 1].SetActive(true);
                            Player.GetComponent<Player>().PutItem();
                            Destroy(Player.GetComponent<Player>().currentPickObject);

                            if (saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves == null)
                                saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves = new List<ModelBoardSave>();

                            saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves.Add(new ModelBoardSave()
                            {
                                typeModelBoard = Player.GetComponent<Player>().currentPickObject.GetComponent<GetItemFromTable>().typeItemFromTable,
                            });

                            Player.GetComponent<Player>().currentPickObject = null;

                            saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave = null;
                            saveManager.UpdatePlayerFile();
                        }
                    }
                }
            }
        }
        IEnumerator WaitAnimTable(float f)
        {
            yield return new WaitForSeconds(f);
            GetComponent<OpenObject>().ObjectAnim = false;
            Workbench.GetComponent<OpenObject>().ArgumentsNotQuit -= 1;
        }
        IEnumerator WaitAnimCamera(float f)
        {
            yield return new WaitForSeconds(f);
            Player.GetComponent<Player>().PutItem();
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
    }
}
