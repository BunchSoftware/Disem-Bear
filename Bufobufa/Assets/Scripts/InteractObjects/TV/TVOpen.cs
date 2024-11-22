using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVOpen : MonoBehaviour
{
    public bool InTrigger = false;
    public bool TVIsOpen = false;
    private bool TVAnim = false;
    private bool ClickedMouse = false;

    private GameObject Player;
    private GameObject Vcam;
    private GameObject TriggerTV;

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
        TriggerTV = transform.Find("TriggerTV").gameObject;
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
                    TriggerTV.SetActive(true);
                }
                else
                {
                    ClickedMouse = false;
                    TriggerTV.SetActive(false);
                }
            }
        }

        if (!Player.GetComponent<PlayerInfo>().PlayerPickSometing && !TVAnim && InTrigger && ClickedMouse && !TVIsOpen)
        {

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



            TVIsOpen = true;
            Player.GetComponent<PlayerInfo>().PlayerInSomething = true;
            TVAnim = true;
            StartCoroutine(WaitAnimBoard(Vcam.GetComponent<MoveAnimation>().TimeAnimation));
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (!TVAnim && TVIsOpen && Input.GetMouseButtonDown(1))
        {
            TriggerTV.SetActive(false);
            TVIsOpen = false;
            Player.GetComponent<PlayerInfo>().PlayerInSomething = false;
            TVAnim = true;
            ClickedMouse = false;
            Vcam.GetComponent<MoveAnimation>().EndMove();
            StartCoroutine(WaitAnimBoard(Vcam.GetComponent<MoveAnimation>().TimeAnimation));
            StartCoroutine(WaitAnimCamera(Vcam.GetComponent<MoveAnimation>().TimeAnimation));

            Player.GetComponent<PlayerMouseMove>().MovePlayer(currentPos);
            Player.GetComponent<PlayerMouseMove>().ReturnPlayerMove();


            GetComponent<BoxCollider>().enabled = true;
        }
    }
    IEnumerator WaitAnimBoard(float f)
    {
        yield return new WaitForSeconds(f);
        TVAnim = false;
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Vcam.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
    }
}
