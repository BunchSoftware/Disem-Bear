using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TableTakesItem : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private List<GetItemFromTable> getItemFromTables;
    public List<PointInfo> pointsInfo = new List<PointInfo>();
    public bool InTrigger = false;
    public bool ClickedMouse = false;
    private GameObject Player;
    public Vector3 ScaleVector;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Count; i++)
        {
            for (int j = 0; j < getItemFromTables.Count; j++)
            {
                if (saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].typeItemFromTable == getItemFromTables[j].typeItemFromTable)
                {
                    GetItemFromTable getItemFromTable = Instantiate(getItemFromTables[j]);
                    getItemFromTable.indexPoint = saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].indexPoint;

                    pointsInfo[getItemFromTable.indexPoint].GetItem = true;
                    pointsInfo[getItemFromTable.indexPoint].obj = getItemFromTable.gameObject;
                    pointsInfo[getItemFromTable.indexPoint].obj.transform.parent = null;
                    pointsInfo[getItemFromTable.indexPoint].obj.transform.position = pointsInfo[getItemFromTable.indexPoint].point.transform.position;
                    pointsInfo[getItemFromTable.indexPoint].obj.GetComponent<MouseTrigger>().enabled = true;
                    pointsInfo[getItemFromTable.indexPoint].obj.GetComponent<BoxCollider>().enabled = true;
                }
            }
        }
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
        if (Player.GetComponent<PlayerInfo>().PlayerPickSometing && InTrigger && ClickedMouse && Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<GetItemFromTable>())
        {
            ClickedMouse = false;
            for (int i = 0; i < pointsInfo.Count; i++)
            {
                if (!pointsInfo[i].GetItem)
                {
                    pointsInfo[i].GetItem = true;
                    pointsInfo[i].obj = Player.GetComponent<PlayerInfo>().currentPickObject;
                    pointsInfo[i].obj.transform.parent = null;
                    Player.GetComponent<PlayerInfo>().PlayerPickSometing = false;
                    Player.GetComponent<PlayerInfo>().currentPickObject = null;
                    pointsInfo[i].obj.transform.position = pointsInfo[i].point.transform.position;
                    pointsInfo[i].obj.GetComponent<GetItemFromTable>().indexPoint = i;
                    pointsInfo[i].obj.GetComponent<MouseTrigger>().enabled = true;
                    pointsInfo[i].obj.GetComponent<BoxCollider>().enabled = true;

                    saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Add(new ItemFromTableSave()
                    {
                        typeItemFromTable = pointsInfo[i].obj.GetComponent<GetItemFromTable>().typeItemFromTable,
                        indexPoint = i
                    });

                    break;
                }
            }
            saveManager.UpdatePlayerFile();
        }
    }

    public void TakeObject(ItemFromTableSave itemFromTableSave)
    {
        for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.Count; i++)
        {
            if (saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves[i].indexPoint == itemFromTableSave.indexPoint)
            {
                saveManager.filePlayer.JSONPlayer.resources.itemFromTableSaves.RemoveAt(i);
                saveManager.UpdatePlayerFile();
                return;
            }
        }
    }

    [System.Serializable]
    public class PointInfo
    {
        public bool GetItem = false;
        public GameObject obj;
        public GameObject point;
    }
}
