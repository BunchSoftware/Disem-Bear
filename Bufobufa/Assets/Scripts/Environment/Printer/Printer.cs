using Game.LPlayer;
using Game.Music;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment.Printer
{
    public class Printer : MonoBehaviour
    {
        public bool InTrigger = false;
        private bool ClickedMouse = false;
        public bool PrinterWork = false;
        public bool ObjectDone = false;
        public GameObject currentObject;
        public List<ObjectInfo> objectInfos = new();

        private GameObject Player;
        private SoundManager SoundManager;
        private Animator animator;
        private ParticleSystem particleSys;

        [SerializeField] Material OrigPrinter;
        [SerializeField] Material DonePrinter;

        [SerializeField] GameObject PrinterImage;

        [SerializeField] private AudioClip VrVrVrVr;

        private void Start()
        {
            particleSys = transform.Find("Particle System").GetComponent<ParticleSystem>();
            animator = GetComponent<Animator>();
            //SoundManager = GameObject.Find("Sound").GetComponent<SoundManager>();
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        public void OnTrigEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                InTrigger = true;
            }
        }
        public void OnTrigExit(Collider other)
        {
            if (other.tag == "Player")
            {
                InTrigger = false;
            }
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("Floor", "ClickedObject")))
                {
                    if (infoHit.collider.gameObject == gameObject)
                    {
                        ClickedMouse = true;
                    }
                    else
                    {
                        ClickedMouse = false;
                    }
                }
            }

            if (!PrinterWork && ClickedMouse && InTrigger && Player.GetComponent<Player>().PlayerPickUpItem && !Player.GetComponent<Player>().PlayerInSomething)
            {
                if (Player.GetComponent<Player>().currentPickObject.GetComponent<PrinterObjectInfo>())
                {
                    for (int i = 0; i < objectInfos.Count; i++)
                    {
                        if (objectInfos[i].NameItemForPrint == Player.GetComponent<Player>().currentPickObject.GetComponent<PrinterObjectInfo>().WhatThis)
                        {
                            ObjectDone = false;
                            PrinterWork = true;
                            Player.GetComponent<Player>().PutItem();
                            Destroy(Player.GetComponent<Player>().currentPickObject);
                            SoundManager.OnPlayOneShot(VrVrVrVr);
                            animator.Play("Printer");
                            particleSys.Play();
                            StartCoroutine(WaitWhilePrintObject(objectInfos[i].TimePrint));
                            currentObject = objectInfos[i].ReturnItem;
                            break;
                        }
                    }
                }
            }
            else if (PrinterWork && ObjectDone && InTrigger && ClickedMouse && !Player.GetComponent<Player>().PlayerPickUpItem)
            {

                PrinterImage.GetComponent<MeshRenderer>().material = OrigPrinter;
                Player.GetComponent<Player>().PickSomething();
                Player.GetComponent<Player>().currentPickObject = Instantiate(currentObject);
                Player.GetComponent<Player>().currentPickObject.GetComponent<MouseTrigger>().enabled = false;
                currentObject = null;
                ClickedMouse = false;
                PrinterWork = false;
                ObjectDone = false;
            }
        }
        IEnumerator WaitWhilePrintObject(float t)
        {
            yield return new WaitForSeconds(t);
            ObjectDone = true;
            PrinterImage.GetComponent<MeshRenderer>().material = DonePrinter;
            particleSys.Stop();
        }
        [System.Serializable]
        public class ObjectInfo
        {
            public string NameItemForPrint;
            public float TimePrint;
            public GameObject ReturnItem;
        }
    }
}
