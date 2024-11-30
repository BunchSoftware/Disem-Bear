using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquariumOpen : MonoBehaviour
{
    private GameObject Player;
    private GameObject AquariumSprite;
    private GameObject Temperature;

    [Header("Координаты куда должен уйти объект при открытии стола(Игрок, камера и сам аквариум)")]
    public Vector3 CoordAquarium = new();
    public Quaternion RotateAquarium = new();
    public float TimeAnimationAquarium = 1f;


    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        AquariumSprite = transform.Find("AquariumSprite").gameObject;
        Temperature = AquariumSprite.transform.Find("Termometr").gameObject;
    }
    private void Update()
    {
        if (!Player.GetComponent<PlayerInfo>().PlayerPickSometing && !GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().InTrigger && !GetComponent<OpenObject>().ObjectIsOpen && GetComponent<OpenObject>().ClickedMouse && !Player.GetComponent<PlayerInfo>().PlayerInSomething)
        {
            AquariumSprite.GetComponent<MoveAnimation>().startCoords = CoordAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().needPosition = true;
            AquariumSprite.GetComponent<MoveAnimation>().startRotate = RotateAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().needRotate = true;
            AquariumSprite.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().StartMove();

            AquariumSprite.GetComponent<BoxCollider>().enabled = true;
            Temperature.GetComponent<BoxCollider>().enabled = true;
        }
        else if (!GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().ObjectIsOpen && Input.GetMouseButtonDown(1))
        {
            AquariumSprite.GetComponent<MoveAnimation>().EndMove();

            AquariumSprite.GetComponent<BoxCollider>().enabled = false;
            Temperature.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
