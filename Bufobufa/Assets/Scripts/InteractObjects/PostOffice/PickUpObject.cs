using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    private GameObject Player;
    public bool falling = true;
    private Vector3 lcScale = new();
    public bool PickUp = false;
    private bool InTrigger = false;

    private void Start()
    {
        Player = GameObject.Find("Player");
        StartCoroutine(NotFalling());
    }
    private void Update()
    {
        if (!falling && !PickUp && InTrigger)
        {
            GetComponent<MouseTrigger>().enabled = false;
            PickUp = true;
            GetComponent<BoxCollider>().enabled = false;
            lcScale = transform.localScale;
            transform.parent = Player.transform;
            transform.localScale = new Vector3(lcScale.x / Player.transform.localScale.x, lcScale.y / Player.transform.localScale.y, lcScale.z / Player.transform.localScale.z);
            Player.GetComponent<PlayerInfo>().PlayerPickSometing = true;
            Player.GetComponent<PlayerInfo>().currentPickObject = gameObject;
            Destroy(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = true;
        }
    }

    
    
    IEnumerator NotFalling()
    {
        yield return new WaitForSeconds(1f);

        falling = false;
    }
}
