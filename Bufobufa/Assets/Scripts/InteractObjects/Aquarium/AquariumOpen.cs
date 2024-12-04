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

    private bool OneTap = true;

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
        if (GetComponent<OpenObject>().ObjectIsOpen && OneTap)
        {
            OneTap = false;
            AquariumSprite.GetComponent<MoveAnimation>().startCoords = CoordAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().needPosition = true;
            AquariumSprite.GetComponent<MoveAnimation>().startRotate = RotateAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().needRotate = true;
            AquariumSprite.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationAquarium;
            AquariumSprite.GetComponent<MoveAnimation>().StartMove();

            AquariumSprite.GetComponent<BoxCollider>().enabled = true;
            Temperature.GetComponent<BoxCollider>().enabled = true;
        }
        else if (!GetComponent<OpenObject>().ObjectIsOpen && !OneTap)
        {
            OneTap = true;
            AquariumSprite.GetComponent<MoveAnimation>().EndMove();

            AquariumSprite.GetComponent<BoxCollider>().enabled = false;
            Temperature.GetComponent<BoxCollider>().enabled = false;

            
        }
        else if (Player.GetComponent<PlayerInfo>().PlayerPickSometing && !GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().InTrigger && GetComponent<OpenObject>().ClickedMouse && !GetComponent<OpenObject>().ObjectIsOpen)
        {
            GetComponent<OpenObject>().ClickedMouse = false;
            if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<MaterialForAquarium>())
            {
                if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<MaterialForAquarium>().nameMaterial == AquariumSprite.GetComponent<Aquarium>().NameMaterial)
                {
                    AquariumSprite.GetComponent<Aquarium>().TimeWaterSpend = Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<MaterialForAquarium>().TimeMaterial;
                    Player.GetComponent<PlayerInfo>().PlayerPickSometing = false;
                    Destroy(Player.GetComponent<PlayerInfo>().currentPickObject);
                    Player.GetComponent<PlayerInfo>().currentPickObject = null;
                }
            }
        }
    }
}
