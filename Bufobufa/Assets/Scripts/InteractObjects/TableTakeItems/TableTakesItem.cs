using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TableTakesItem : MonoBehaviour
{
    public List<pointInfo> points = new List<pointInfo>();
    public bool InTrigger = false;
    public bool ClickedMouse = false;
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
            for (int i = 0; i < points.Count; i++)
            {
                if (!points[i].GetItem)
                {
                    points[i].GetItem = true;
                    points[i].obj = Player.GetComponent<PlayerInfo>().currentPickObject;
                    points[i].obj.transform.parent = transform;
                    Player.GetComponent<PlayerInfo>().PlayerPickSometing = false;
                    Player.GetComponent<PlayerInfo>().currentPickObject = null;
                    points[i].obj.transform.position = points[i].point.transform.position;
                    break;
                }
            }
        }
    }
    [System.Serializable]
    public class pointInfo
    {
        public bool GetItem = false;
        public GameObject obj;
        public GameObject point;
    }
}
