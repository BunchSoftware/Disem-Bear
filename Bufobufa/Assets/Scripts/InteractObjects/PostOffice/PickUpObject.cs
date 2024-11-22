using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    private GameObject Player;
    public bool Clicked = false;
    public bool falling = true;
    [SerializeField] private Vector3 positionItem = new();
    private Vector3 lcScale = new();
    public bool PickUp = false;
    private float timer = 0f;
    private void Start()
    {
        Player = GameObject.Find("Player");
        StartCoroutine(NotFalling());
    }
    private void OnMouseDown()
    {
        Clicked = true;
    }
    private void Update()
    {
        if (PickUp && timer < 1f)
        {
            timer += Time.deltaTime;
            transform.localScale = new Vector3(lcScale.x / Player.transform.localScale.x, lcScale.y / Player.transform.localScale.y, lcScale.z / Player.transform.localScale.z);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!falling && Clicked)
        {
            PickUp = true;
            GetComponent<BoxCollider>().enabled = false;
            lcScale = transform.localScale;
            transform.parent = Player.transform;
            transform.localScale = new Vector3(lcScale.x / Player.transform.localScale.x, lcScale.y / Player.transform.localScale.y, lcScale.z / Player.transform.localScale.z);
            transform.localPosition = positionItem;
            Player.GetComponent<PlayerInfo>().PlayerPickSometing = true;
            Player.GetComponent<PlayerInfo>().currentPickObject = gameObject;
        }
    }
    
    IEnumerator NotFalling()
    {
        yield return new WaitForSeconds(1f);
        falling = false;
    }
}
