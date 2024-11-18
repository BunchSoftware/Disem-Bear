using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquariumOpen : MonoBehaviour
{
    public GameObject DisplayCount;
    public bool InTrigger = false;
    private bool AquariumIsOpen = false;

    private GameObject Player;
    private GameObject Vcam;
    private GameObject AquariumSprite;
    private GameObject Temperature;

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


    private void Start()
    {
        Vcam = GameObject.FindGameObjectWithTag("Vcam");
        Player = GameObject.FindGameObjectWithTag("Player");
        AquariumSprite = transform.Find("AquariumSprite").gameObject;
        DisplayCount = transform.Find("DisplayCount").gameObject;
        Temperature = AquariumSprite.transform.Find("Termometr").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = true;
            DisplayCount.GetComponent<Animator>().SetBool("On", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = false;
            DisplayCount.GetComponent<Animator>().SetBool("On", true);
        }
    }
    private void Update()
    {
        if (InTrigger && !AquariumIsOpen && Input.GetKeyDown(KeyCode.E))
        {
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

            AquariumSprite.GetComponent<MoveAnimation>().startCoords = CoordAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().needPosition = true;
            AquariumSprite.GetComponent<MoveAnimation>().startRotate = RotateAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().needRotate = true;
            AquariumSprite.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().StartMove();


            AquariumIsOpen = true;
            AquariumSprite.GetComponent<Aquarium>().AquariumOpen = true;
            Temperature.GetComponent<Temperature>().AquariumOpen = true;
        }
        else if (AquariumIsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            Vcam.GetComponent<MoveAnimation>().EndMove();
            StartCoroutine(WaitAnimCamera(Vcam.GetComponent<MoveAnimation>().TimeAnimation));

            Player.GetComponent<MoveAnimation>().EndMove();
            Player.GetComponent<MovePlayer>().ReturnMovePlayer();

            AquariumSprite.GetComponent<MoveAnimation>().EndMove();

            AquariumIsOpen = false;
            AquariumSprite.GetComponent<Aquarium>().AquariumOpen = false;
            Temperature.GetComponent<Temperature>().AquariumOpen = false;
        }
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Vcam.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
    }
}
