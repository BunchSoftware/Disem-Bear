using Cinemachine;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOpen : MonoBehaviour
{
    public GameObject DisplayCount;
    public bool InTrigger = false;
    public bool TableIsOpen = false;

    [SerializeField] GameObject Damper;
    private GameObject Player;
    private GameObject Vcam;

    [Header("Координаты куда должен уйти объект при открытии стола(Игрок и камера)")]
    public Vector3 CoordPlayer = new();
    public float TimeAnimationPlayer = 1f;
    public Vector3 CoordVcam = new();
    public Quaternion RotateVcam = new();
    public float TimeAnimationVcam = 1f;


    private void Start()
    {
        Vcam = GameObject.FindGameObjectWithTag("Vcam");
        Player = GameObject.FindGameObjectWithTag("Player");
        DisplayCount = transform.Find("DisplayCount").gameObject;
    }
    public void OnTrigEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = true;
            DisplayCount.GetComponent<Animator>().SetBool("On", true);
        }
    }
    public void OnTrigExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = false;
            DisplayCount.GetComponent<Animator>().SetBool("On", false);
        }
    }
    private void Update()
    {
        if (InTrigger && Input.GetKeyDown(KeyCode.E) && !TableIsOpen){
            DisplayCount.GetComponent<Animator>().SetBool("On", false);


            Vcam.GetComponent<CinemachineVirtualCamera>().Follow = null;
            Vcam.GetComponent<MoveAnimation>().startCoords = CoordVcam;
            Vcam.GetComponent<MoveAnimation>().needPosition = true;
            Vcam.GetComponent<MoveAnimation>().startRotate = RotateVcam;
            Vcam.GetComponent<MoveAnimation>().needRotate = true;
            Vcam.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationVcam;
            Vcam.GetComponent<MoveAnimation>().StartMove();


            Player.GetComponent<MovePlayer>().StopMovePlayer();
            Player.GetComponent<MoveAnimation>().startCoords = CoordPlayer;
            Player.GetComponent<MoveAnimation>().needPosition = true;
            Player.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationPlayer;
            Player.GetComponent<MoveAnimation>().StartMove();


            TableIsOpen = true;
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (TableIsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            Vcam.GetComponent<MoveAnimation>().EndMove();
            StartCoroutine(WaitAnimCamera(Vcam.GetComponent<MoveAnimation>().TimeAnimation));

            Player.GetComponent<MoveAnimation>().EndMove();
            Player.GetComponent<MovePlayer>().ReturnMovePlayer();

            TableIsOpen = false;
            GetComponent<BoxCollider>().enabled = true;
        }
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Vcam.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
    }
}
