using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTakesItem : MonoBehaviour
{
    public List<GameObject> points = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();
    private bool InTrigger = false;
    private bool ClickedMouse = false;
    private GameObject Player;
    public Vector3 ScaleVector;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
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
        if (Player.GetComponent<PlayerInfo>().PlayerPickSometing && InTrigger && ClickedMouse)
        {
            ClickedMouse = false;
            if (items.Count < points.Count)
            {
                items.Add(Player.GetComponent<PlayerInfo>().currentPickObject);
                items[items.Count - 1].transform.parent = transform;
                items[items.Count - 1].transform.localPosition = points[items.Count - 1].transform.localPosition;
                items[items.Count - 1].transform.localScale = ScaleVector;
                items[items.Count - 1].GetComponent<GetItemFromTable>().TriggerObject.transform.localRotation = points[items.Count - 1].transform.localRotation;
                items[items.Count - 1].GetComponent<MouseTrigger>().enabled = true;
                items[items.Count - 1].GetComponent<BoxCollider>().enabled = true;
                items[items.Count - 1].GetComponent<GetItemFromTable>().enabled = true;
                Player.GetComponent<PlayerInfo>().PlayerPickSometing = false;
                Player.GetComponent<PlayerInfo>().currentPickObject = null;
            }
        }
    }
}
