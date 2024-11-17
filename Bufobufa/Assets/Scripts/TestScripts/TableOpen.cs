using Cinemachine;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOpen : MonoBehaviour
{
    public GameObject DisplayCount;
    public bool InTrigger = false;
    private bool TableIsOpen = false;

    [SerializeField] GameObject Damper;
    private GameObject Player;
    private GameObject Vcam;

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
            Vcam.GetComponent<CinemachineVirtualCamera>().Follow = null;
            Vcam.GetComponent<Animator>().SetBool("On", true);
            Player.GetComponent<TableOpenAnim>().StartMove();
            TableIsOpen = true;
            Damper.SetActive(false);
        }
        else if (TableIsOpen && Input.GetKeyDown(KeyCode.E))
        {
            Vcam.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
            Vcam.GetComponent<Animator>().SetBool("On", false);
            Player.GetComponent<MovePlayer>().ReturnMovePlayer();
            TableIsOpen = false;
            Damper.SetActive(true);
        }
    }
}
