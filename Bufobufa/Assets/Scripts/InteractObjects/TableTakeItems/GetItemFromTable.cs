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
    public int numPoint = 0;
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
            if (Physics.Raycast(ray, out var infoHit))
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
        if (InTrigger && ClickedMouse && !Player.GetComponent<PlayerInfo>().PlayerPickSometing && !Player.GetComponent<PlayerInfo>().PlayerInSomething)
        {

            //GameObject tmp = transform.parent.GetComponent<TableTakesItem>().points[transform.parent.GetComponent<TableTakesItem>().items.IndexOf(gameObject)];
            //transform.parent.GetComponent<TableTakesItem>().points.Remove(tmp);
            //transform.parent.GetComponent<TableTakesItem>().points.Add(tmp);
            //transform.parent.GetComponent<TableTakesItem>().items.Remove(gameObject);
            Player.GetComponent<PlayerInfo>().PlayerPickSometing = true;
            Player.GetComponent<PlayerInfo>().currentPickObject = table.GetComponent<TableTakesItem>().pointsInfo[numPoint].obj;
            table.GetComponent<TableTakesItem>().pointsInfo[numPoint].GetItem = false;
            //transform.parent = Player.transform;

        }
    }
}
