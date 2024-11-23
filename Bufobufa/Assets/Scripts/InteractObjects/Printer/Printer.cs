using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer : MonoBehaviour
{
    public bool InTrigger = false;
    private bool ClickedMouse = false;
    public bool PrinterWork = false;
    public bool ObjectDone = false;
    public GameObject currentObject;
    public List<ObjectInfo> objectInfos = new();

    private GameObject Player;
    private GameObject TriggerPrinter;

    [SerializeField] DialogManager Dialog; //”ƒ¿À»“‹
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        TriggerPrinter = transform.Find("TriggerPrinter").gameObject;
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
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("Floor", "ClickedObject")))
            {
                if (infoHit.collider.gameObject == gameObject)
                {
                    ClickedMouse = true;
                    TriggerPrinter.SetActive(true);
                }
                else
                {
                    ClickedMouse = false;
                    TriggerPrinter.SetActive(false);
                }
            }
        }

        if (!PrinterWork && ClickedMouse && InTrigger && Player.GetComponent<PlayerInfo>().PlayerPickSometing && !Player.GetComponent<PlayerInfo>().PlayerInSomething)
        {
            InTrigger = false;
            if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PrinterObjectInfo>())
            {
                for (int i = 0; i < objectInfos.Count; i++)
                {
                    if (objectInfos[i].NameItemForPrint == Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PrinterObjectInfo>().WhatThis)
                    {
                        ObjectDone = false;
                        PrinterWork = true;
                        Player.GetComponent<PlayerInfo>().PlayerPickSometing = false;
                        Destroy(Player.GetComponent<PlayerInfo>().currentPickObject);
                        StartCoroutine(WaitWhilePrintObject(objectInfos[i].TimePrint));
                        currentObject = objectInfos[i].ReturnItem;
                        break;
                    }
                }
            }
        }
        else if (PrinterWork && ObjectDone && InTrigger && ClickedMouse && !Player.GetComponent<PlayerInfo>().PlayerPickSometing)
        {
            Dialog.RunConditionSkip("PrinterWork"); //”ƒ¿À»“‹
            Player.GetComponent<PlayerInfo>().PlayerPickSometing = true;
            Player.GetComponent<PlayerInfo>().currentPickObject = Instantiate(currentObject);
            currentObject = null;
            ClickedMouse = false;
            PrinterWork = false;
            ObjectDone = false;
        }
    }
    IEnumerator WaitWhilePrintObject(float t)
    {
        yield return new WaitForSeconds(t);
        ObjectDone = true;
    }
    [System.Serializable]
    public class ObjectInfo
    {
        public string NameItemForPrint;
        public float TimePrint;
        public GameObject ReturnItem;
    }
}
