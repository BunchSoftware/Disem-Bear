using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class NextRoom : MonoBehaviour
{
    public bool PlayerLeft = true;
    [SerializeField] BoxCollider LeftRoomCollider;
    [SerializeField] BoxCollider RightRoomCollider;
    private GameObject Player;
    private GameObject Vcam;

    [Header("Координаты куда должен уйти объект при открытии стола(Игрок и камера)")]
    public Vector3 CoordPlayer = new();
    public float TimeAnimationPlayer = 1f;

    private Vector3 currentPos;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Vcam = GameObject.FindGameObjectWithTag("Vcam");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vcam.GetComponent<CinemachineConfiner>().m_BoundingVolume = null;

            Player.GetComponent<PlayerMouseMove>().MovePlayer(CoordPlayer);
            Player.GetComponent<PlayerMouseMove>().StopPlayerMove();
        }
    }
}
