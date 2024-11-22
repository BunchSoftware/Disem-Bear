using Cinemachine;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOpen : MonoBehaviour
{
    public bool InTrigger = false;
    public bool TableIsOpen = false;
    private bool TableAnim = false;
    private bool ClickedMouse = false;

    private GameObject Player;
    private GameObject Vcam;
    private GameObject TriggerTable;

    [Header("Координаты куда должен уйти объект при открытии стола(Игрок и камера)")]
    public Vector3 CoordPlayer = new();
    public float TimeAnimationPlayer = 1f;
    public Vector3 CoordVcam = new();
    public Quaternion RotateVcam = new();
    public float TimeAnimationVcam = 1f;

    private Vector3 currentPos = new();


    private void Start()
    {
        Vcam = GameObject.FindGameObjectWithTag("Vcam");
        Player = GameObject.FindGameObjectWithTag("Player");
        TriggerTable = transform.Find("TriggerTable").gameObject;
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
                    TriggerTable.SetActive(true);
                }
                else
                {
                    ClickedMouse = false;
                    TriggerTable.SetActive(false);
                }
            }
        }


        if (!Player.GetComponent<PlayerInfo>().PlayerPickSometing && !TableAnim && InTrigger && ClickedMouse && !TableIsOpen){

            ClickedMouse = false;
            Vcam.GetComponent<CinemachineVirtualCamera>().Follow = null;
            Vcam.GetComponent<MoveAnimation>().startCoords = CoordVcam;
            Vcam.GetComponent<MoveAnimation>().needPosition = true;
            Vcam.GetComponent<MoveAnimation>().startRotate = RotateVcam;
            Vcam.GetComponent<MoveAnimation>().needRotate = true;
            Vcam.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationVcam;
            Vcam.GetComponent<MoveAnimation>().StartMove();


            currentPos = Player.transform.position;
            Player.GetComponent<PlayerMouseMove>().MovePlayer(CoordPlayer);
            Player.GetComponent<PlayerMouseMove>().StopPlayerMove();
            


            TableIsOpen = true;
            Player.GetComponent<PlayerInfo>().PlayerInSomething = true;
            TableAnim = true;
            StartCoroutine(WaitAnimTable(Vcam.GetComponent<MoveAnimation>().TimeAnimation));
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (!TableAnim && TableIsOpen && Input.GetMouseButtonDown(1))
        {
            TriggerTable.SetActive(false);
            TableIsOpen = false;
            Player.GetComponent<PlayerInfo>().PlayerInSomething = false;
            TableAnim = true;
            ClickedMouse = false;
            Vcam.GetComponent<MoveAnimation>().EndMove();
            StartCoroutine(WaitAnimTable(Vcam.GetComponent<MoveAnimation>().TimeAnimation));
            StartCoroutine(WaitAnimCamera(Vcam.GetComponent<MoveAnimation>().TimeAnimation));

            Player.GetComponent<PlayerMouseMove>().MovePlayer(currentPos);
            Player.GetComponent<PlayerMouseMove>().ReturnPlayerMove();

            
            GetComponent<BoxCollider>().enabled = true;
        }
    }
    IEnumerator WaitAnimTable(float f)
    {
        yield return new WaitForSeconds(f);
        TableAnim = false;
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Vcam.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
    }
}
