using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GetItemFromTable : MonoBehaviour
{
    private GameObject Player;
    public string typeItemFromTable;
    public bool InTrigger = false;
    public bool ClickedMouse = false;
    public bool isTube = false;
    public int indexPoint = 0;
    private Transform table;


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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = false;
        }
    }

    private void Start()
    {
        Player = GameObject.Find("Player");
        table = GameObject.Find("minitable").transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("ClickedObject")))
            {
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
        if (InTrigger && ClickedMouse && !Player.GetComponent<PlayerInfo>().playerPickSometing && !Player.GetComponent<PlayerInfo>().playerInSomething)
        {
            Player.GetComponent<PlayerInfo>().playerPickSometing = true;
            Player.GetComponent<PlayerInfo>().currentPickObject = table.GetComponent<TableTakesItem>().pointsInfo[indexPoint].obj;
            table.GetComponent<TableTakesItem>().pointsInfo[indexPoint].GetItem = false;
            table.GetComponent<TableTakesItem>().pointsInfo[indexPoint].obj = null;


            bool isReturn = table.GetComponent<TableTakesItem>().TakeObject(new ItemFromTableSave()
            {
                typeItemFromTable = typeItemFromTable,
                indexPoint = indexPoint,
            });


            if (isReturn)
                return;
        }
        else if (isTube && InTrigger)
        {
            table.GetComponent<TableTakesItem>().saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave = new ItemFromTableSave()
            {
                typeItemFromTable = typeItemFromTable,
                indexPoint = indexPoint,
            };
            table.GetComponent<TableTakesItem>().saveManager.UpdatePlayerFile();
            isTube = false;
        }
    }
}
