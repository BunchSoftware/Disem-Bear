using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquariumOpen : MonoBehaviour
{
    public bool InTrigger = false;
    private bool AquariumIsOpen = false;
    private bool AquariumAnim = false;
    private bool ClickedMouse = false;

    private GameObject Player;
    private GameObject Vcam;
    private GameObject AquariumSprite;
    private GameObject Temperature;
    private GameObject TriggerAquarium;

    [Header("Координаты куда должен уйти объект при открытии стола(Игрок, камера и сам аквариум)")]
    public Vector3 CoordPlayer = new();
    public Quaternion RotatePlayer = new();
    public float TimeAnimationPlayer = 1f;

    public Vector3 CoordVcam = new();
    public Quaternion RotateVcam = new();
    public float TimeAnimationVcam = 1f;

    public Vector3 CoordAquarium = new();
    public Quaternion RotateAquarium = new();
    public float TimeAnimationAquarium = 1f;

    private Vector3 currentPos = new();



    private void Start()
    {

        Vcam = GameObject.FindGameObjectWithTag("Vcam");
        Player = GameObject.FindGameObjectWithTag("Player");
        AquariumSprite = transform.Find("AquariumSprite").gameObject;
        Temperature = AquariumSprite.transform.Find("Termometr").gameObject;
        TriggerAquarium = transform.Find("TriggerAquarium").gameObject;
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
                    TriggerAquarium.SetActive(true);
                }
                else
                {
                    ClickedMouse = false;
                    TriggerAquarium.SetActive(false);
                }
            }
        }


        if (!Player.GetComponent<PlayerInfo>().PlayerPickSometing && !AquariumAnim && InTrigger && !AquariumIsOpen && ClickedMouse && !Player.GetComponent<PlayerInfo>().PlayerInSomething)
        {

            AquariumIsOpen = true;
            Player.GetComponent<PlayerInfo>().PlayerInSomething = true;
            AquariumAnim = true;
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

            AquariumSprite.GetComponent<MoveAnimation>().startCoords = CoordAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().needPosition = true;
            AquariumSprite.GetComponent<MoveAnimation>().startRotate = RotateAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().needRotate = true;
            AquariumSprite.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().StartMove();


            StartCoroutine(WaitAnimAquarium(Vcam.GetComponent<MoveAnimation>().TimeAnimation + 0.1f));
            AquariumSprite.GetComponent<BoxCollider>().enabled = true;
            Temperature.GetComponent<BoxCollider>().enabled = true;
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (!AquariumAnim && AquariumIsOpen && Input.GetMouseButtonDown(1))
        {
            TriggerAquarium.SetActive(false);
            AquariumIsOpen = false;
            AquariumAnim = true;
            Vcam.GetComponent<MoveAnimation>().EndMove();
            StartCoroutine(WaitAnimAquarium(Vcam.GetComponent<MoveAnimation>().TimeAnimation + 0.1f));
            StartCoroutine(WaitAnimCamera(Vcam.GetComponent<MoveAnimation>().TimeAnimation + 0.1f));

            Player.GetComponent<PlayerMouseMove>().MovePlayer(currentPos);
            Player.GetComponent<PlayerMouseMove>().ReturnPlayerMove();

            AquariumSprite.GetComponent<MoveAnimation>().EndMove();


            AquariumSprite.GetComponent<BoxCollider>().enabled = false;
            Temperature.GetComponent<BoxCollider>().enabled = false;
            GetComponent<BoxCollider>().enabled = true;
        }
    }
    IEnumerator WaitAnimAquarium(float f)
    {
        yield return new WaitForSeconds(f);
        AquariumAnim = false;
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Vcam.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
        Player.GetComponent<PlayerInfo>().PlayerInSomething = false;
    }
}
