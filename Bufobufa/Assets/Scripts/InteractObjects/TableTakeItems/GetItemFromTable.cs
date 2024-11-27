using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItemFromTable : MonoBehaviour
{
    private GameObject Player;
    public bool InTrigger = false;
    public bool ClickedMouse = false;

    public GameObject TriggerObject;

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
        TriggerObject = transform.Find("TriggerPackage").gameObject;
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
                    TriggerObject.SetActive(true);
                }
                else
                {
                    ClickedMouse = false;
                    TriggerObject.SetActive(false);
                }
            }
        }
        if (InTrigger && ClickedMouse && !Player.GetComponent<PlayerInfo>().PlayerPickSometing && !Player.GetComponent<PlayerInfo>().PlayerInSomething)
        {
            GameObject tmp = transform.parent.GetComponent<TableTakesItem>().points[transform.parent.GetComponent<TableTakesItem>().items.IndexOf(gameObject)];
            transform.parent.GetComponent<TableTakesItem>().points.Remove(tmp);
            transform.parent.GetComponent<TableTakesItem>().points.Add(tmp);
            transform.parent.GetComponent<TableTakesItem>().items.Remove(gameObject);
            Player.GetComponent<PlayerInfo>().PlayerPickSometing = true;
            Player.GetComponent<PlayerInfo>().currentPickObject = gameObject;
            transform.parent = Player.transform;

        }
    }
}
