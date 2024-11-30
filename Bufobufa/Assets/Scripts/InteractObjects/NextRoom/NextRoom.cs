using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class NextRoom : MonoBehaviour
{
    private GameObject Player;
    private GameObject Vcam;

    [Header("Координаты куда должен уйти игрок")]
    public Vector3 CoordPlayer = new();
    public float TimeAnimationPlayer = 1f;
    public Vector3 CoordVcam = new();
    public float TimeAnimationVcam = 1f;

    [SerializeField] private BoxCollider RightRoomStrelka;
    private GameObject InvWallBetweenRooms;

    private Vector3 currentPos;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Vcam = GameObject.FindGameObjectWithTag("Vcam");
        InvWallBetweenRooms = GameObject.Find("InvWallBetweenRooms");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InvWallBetweenRooms.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(WaitBakeMesh(0.01f));
            Player.GetComponent<PlayerMouseMove>().StopPlayerMove();

            Vcam.GetComponent<MoveCameraAnimation>().startCoords = CoordVcam;
            Vcam.GetComponent<MoveCameraAnimation>().needPosition = true;
            Vcam.GetComponent<MoveCameraAnimation>().needRotate = false;
            Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation = TimeAnimationVcam;
            Vcam.GetComponent<MoveCameraAnimation>().StartMove();
            StartCoroutine(WaitAnimCamera(TimeAnimationVcam));
        }
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Player.GetComponent<PlayerMouseMove>().ReturnPlayerMove();
        InvWallBetweenRooms.SetActive(true);
        RightRoomStrelka.enabled = true;
    }
    IEnumerator WaitBakeMesh(float f)
    {
        yield return new WaitForSeconds(f);
        Player.GetComponent<PlayerMouseMove>().MovePlayer(CoordPlayer);
    }
}
