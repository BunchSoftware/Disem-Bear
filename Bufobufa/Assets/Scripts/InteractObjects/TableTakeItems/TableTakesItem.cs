using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TableTakesItem : MonoBehaviour
{
    public SaveManager saveManager;
    [SerializeField] private List<GetItemFromTable> getItemFromTables;
    public List<PointInfo> pointsInfo = new List<PointInfo>();
    public bool InTrigger = false;
    public bool ClickedMouse = false;
    private GameObject Player;
    public Vector3 ScaleVector;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

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

        //                pointsInfo[getItemFromTable.indexPoint].GetItem = true;
        //                pointsInfo[getItemFromTable.indexPoint].obj = getItemFromTable.gameObject;
        //                pointsInfo[getItemFromTable.indexPoint].obj.transform.parent = null;
        //                pointsInfo[getItemFromTable.indexPoint].obj.transform.position = pointsInfo[getItemFromTable.indexPoint].point.transform.position;
        //                pointsInfo[getItemFromTable.indexPoint].obj.GetComponent<MouseTrigger>().enabled = true;
        //                pointsInfo[getItemFromTable.indexPoint].obj.GetComponent<BoxCollider>().enabled = true;
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
        //                Player.GetComponent<PlayerInfo>().PlayerPickSometing = true;
        //                Player.GetComponent<PlayerInfo>().currentPickObject = getItemFromTable.gameObject;
        //            }
        //        }
        //    }
        //}
    }
    public void OnTrigEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = true;
        }
    }
    public void OnTrigExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = false;
        }
    }
    public GameObject wtf;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("ClickedObject")))
            {
                wtf = infoHit.collider.gameObject;
                if (infoHit.collider.gameObject == gameObject)
                {
                    ClickedMouse = true;
                }
                else
                {
                    ClickedMouse = false;
                }
            }
        }
        if (Player.GetComponent<PlayerInfo>().playerPickSometing && InTrigger && ClickedMouse && Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<GetItemFromTable>())
        {
            ClickedMouse = false;
            for (int i = 0; i < pointsInfo.Count; i++)
            {
                if (pointsInfo[i].GetItem == false)
                {
                    pointsInfo[i].GetItem = true;
                    pointsInfo[i].obj = Player.GetComponent<PlayerInfo>().currentPickObject;
                    pointsInfo[i].obj.transform.parent = null;
                    Player.GetComponent<PlayerInfo>().playerPickSometing = false;
                    Player.GetComponent<PlayerInfo>().currentPickObject = null;
                    pointsInfo[i].obj.transform.position = pointsInfo[i].point.transform.position;
                    pointsInfo[i].obj.GetComponent<GetItemFromTable>().indexPoint = i;
                    pointsInfo[i].obj.GetComponent<MouseTrigger>().enabled = true;
                    pointsInfo[i].obj.GetComponent<BoxCollider>().enabled = true;

                    if (saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves == null)
                        saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves = new List<ItemFromTableSave>();

                    saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Add(new ItemFromTableSave()
                    {
                        typeItemFromTable = pointsInfo[i].obj.GetComponent<GetItemFromTable>().typeItemFromTable,
                        indexPoint = i
                    });

                    saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave = null;
                    saveManager.UpdatePlayerFile();
                    break;
                }
            }
        }
    }

    public bool TakeObject(ItemFromTableSave itemFromTableSave)
    {
        for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Count; i++)
        {
            if (saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].indexPoint == itemFromTableSave.indexPoint)
            {
                saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.RemoveAt(i);
                saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave = new ItemFromTableSave() 
                { 
                    typeItemFromTable = itemFromTableSave.typeItemFromTable,
                    indexPoint = itemFromTableSave.indexPoint,
                };
                saveManager.UpdatePlayerFile();
                return true;
            }
        }
        return false;
    }

    [System.Serializable]
    public class PointInfo
    {
        public bool GetItem = false;
        public GameObject obj;
        public GameObject point;
    }
}
