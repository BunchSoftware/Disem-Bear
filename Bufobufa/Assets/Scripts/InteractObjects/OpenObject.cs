using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class OpenObject : MonoBehaviour
{
    public bool InTrigger = false;
    public bool ObjectIsOpen = false;
    public bool ObjectAnim = false;
    public bool ClickedMouse = false;

    private GameObject Player;
    private GameObject Vcam;
    private BoxCollider ColliderVCRoom1;
    private GameObject TriggerObject;
    private GameObject MainCamera;


    [Header("Координаты куда должен уйти объект при открытии стола(Игрок и камера)")]
    public Vector3 CoordPlayer = new();
    public float TimeAnimationPlayer = 1f;
    public Vector3 CoordVcam = new();
    public Quaternion RotateVcam = new();
    public float TimeAnimationVcam = 1f;

    private Vector3 currentPosPlayer = new();


    private void Start()
    {
        Vcam = GameObject.FindGameObjectWithTag("Vcam");
        ColliderVCRoom1 = GameObject.Find("ColliderVCRoom1").GetComponent<BoxCollider>();
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Player = GameObject.FindGameObjectWithTag("Player");
        TriggerObject = transform.Find("TriggerObject").gameObject;
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
                    TriggerObject.SetActive(true);
                }
                else
                {
                    ClickedMouse = false;
                    TriggerObject.SetActive(false);
                }
            }
        }


        if (!Player.GetComponent<PlayerInfo>().PlayerPickSometing && !ObjectAnim && InTrigger && ClickedMouse && !ObjectIsOpen && !Player.GetComponent<PlayerInfo>().PlayerInSomething)
        {
            ClickedMouse = false;

            Vcam.GetComponent<CinemachineVirtualCamera>().Follow = null;
            var tmpPosCamera = MainCamera.transform.position;
            Vcam.GetComponent<CinemachineConfiner>().m_BoundingVolume = null;
            Vcam.transform.position = tmpPosCamera;
            Vcam.GetComponent<MoveCameraAnimation>().startCoords = CoordVcam;
            Vcam.GetComponent<MoveCameraAnimation>().needPosition = true;
            Vcam.GetComponent<MoveCameraAnimation>().startRotate = RotateVcam;
            Vcam.GetComponent<MoveCameraAnimation>().needRotate = true;
            Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation = TimeAnimationVcam;
            Vcam.GetComponent<MoveCameraAnimation>().StartMove();
            

            currentPosPlayer = Player.transform.position;
            Player.GetComponent<PlayerMouseMove>().MovePlayer(CoordPlayer);
            Player.GetComponent<PlayerMouseMove>().StopPlayerMove();



            ObjectIsOpen = true;
            Player.GetComponent<PlayerInfo>().PlayerInSomething = true;
            ObjectAnim = true;
            StartCoroutine(WaitAnimTable(Vcam.GetComponent<MoveAnimation>().TimeAnimation + 0.1f));
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (!ObjectAnim && ObjectIsOpen && Input.GetMouseButtonDown(1))
        {
            TriggerObject.SetActive(false);
            ObjectIsOpen = false;
            ObjectAnim = true;
            ClickedMouse = false;

            Vcam.GetComponent<MoveCameraAnimation>().EndMove();

            StartCoroutine(WaitAnimTable(Vcam.GetComponent<MoveAnimation>().TimeAnimation + 0.1f));
            StartCoroutine(WaitAnimCamera(Vcam.GetComponent<MoveAnimation>().TimeAnimation + 0.1f));

            Player.GetComponent<PlayerMouseMove>().MovePlayer(currentPosPlayer);
            Player.GetComponent<PlayerMouseMove>().ReturnPlayerMove();

            GetComponent<BoxCollider>().enabled = true;
        }
    }
    IEnumerator WaitAnimTable(float f)
    {
        yield return new WaitForSeconds(f);
        ObjectAnim = false;
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Vcam.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
        Vcam.GetComponent<CinemachineConfiner>().m_BoundingVolume = ColliderVCRoom1;
        Player.GetComponent<PlayerInfo>().PlayerInSomething = false;
    }
}
