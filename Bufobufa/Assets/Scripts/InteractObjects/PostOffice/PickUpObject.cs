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
    private Vector3 lcScale = new();
    public bool PickUp = false;
    private float timer = 0f;
    [SerializeField] DialogManager Dialog; //”ƒ¿À»“‹
    public string CodeWord = ""; //”ƒ¿À»“‹
    [SerializeField] GameObject OffStrelka;
    [SerializeField] GameObject OnStrelka;
    private void Start()
    {
        Dialog = GameObject.Find("DialogManager").GetComponent<DialogManager>(); //”ƒ¿À»“‹
        OffStrelka = GameObject.Find("Strelki").transform.Find("Strelka").gameObject; //”ƒ¿À»“‹
        OnStrelka = GameObject.Find("Strelki").transform.Find("Strelka (1)").gameObject; //”ƒ¿À»“‹

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
        if (collision.transform.tag == "Player")
        {
            if (!falling && Clicked && !PickUp)
            {
                Dialog.RunConditionSkip(CodeWord); //”ƒ¿À»“‹
                OffStrelka.SetActive(false); //”ƒ¿À»“‹
                OnStrelka.SetActive(true); //”ƒ¿À»“‹

                GetComponent<MouseTrigger>().enabled = false;
                PickUp = true;
                GetComponent<BoxCollider>().enabled = false;
                lcScale = transform.localScale;
                transform.parent = Player.transform;
                transform.localScale = new Vector3(lcScale.x / Player.transform.localScale.x, lcScale.y / Player.transform.localScale.y, lcScale.z / Player.transform.localScale.z);
                Player.GetComponent<PlayerInfo>().PlayerPickSometing = true;
                Player.GetComponent<PlayerInfo>().currentPickObject = gameObject;
            }
        }
    }
    
    IEnumerator NotFalling()
    {
        yield return new WaitForSeconds(1f);
        falling = false;
    }
}
