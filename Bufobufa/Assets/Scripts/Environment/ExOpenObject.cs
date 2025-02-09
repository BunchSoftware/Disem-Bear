using Game.Environment;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace A
{
    public class ExOpenObject : MonoBehaviour
    {
        public UnityEvent ApperanceAnimationTV;
        public UnityEvent DisapperanceAnimationTV;

        public bool InTrigger = false;
        public bool ObjectIsOpen = false;
        public bool ObjectAnim = false;
        public bool ClickedMouse = false;

        public int ArgumentsNotQuit = 0;

        private GameObject Player;
        public GameObject Vcam;
        public GameObject TriggerObject;
        private GameObject MainCamera;


        [Header("Координаты куда должен уйти объект при открытии стола(Игрок и камера)")]
        public Vector3 CoordPlayer = new();
        public float TimeAnimationPlayer = 1f;
        public Vector3 CoordVcam = new();
        public Quaternion RotateVcam = new();
        public float TimeAnimationVcam = 1f;

        public Vector3 currentPosPlayer = new();
        private Vector3 currentPosVcam = new();
        private Quaternion currentRotVcam = new();


        private void Start()
        {
            Vcam = GameObject.FindGameObjectWithTag("Vcam");
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            Player = GameObject.FindGameObjectWithTag("Player");
            TriggerObject = transform.Find("TriggerObject").gameObject;
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
                    if (infoHit.collider.gameObject == gameObject && !ObjectIsOpen)
                    {
                        ClickedMouse = true;
                    }
                    else
                    {
                        ClickedMouse = false;
                    }
                }
            }


            if (!Player.GetComponent<Player>().PlayerPickUpItem && !ObjectAnim && InTrigger && ClickedMouse && !ObjectIsOpen)
            {


                var tmpPosCamera = MainCamera.transform.position;
                Vcam.transform.position = tmpPosCamera;
                currentPosVcam = tmpPosCamera;
                currentRotVcam = Vcam.transform.rotation;

                //Vcam.GetComponent<MoveCameraAnimation>().startCoords = CoordVcam;
                //Vcam.GetComponent<MoveCameraAnimation>().needPosition = true;
                //Vcam.GetComponent<MoveCameraAnimation>().startRotate = RotateVcam;
                //Vcam.GetComponent<MoveCameraAnimation>().needRotate = true;
                //Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation = TimeAnimationVcam;
                //Vcam.GetComponent<MoveCameraAnimation>().StartMove();


                currentPosPlayer = Player.transform.position;
                Player.GetComponent<PlayerMouseMove>().MovePlayer(CoordPlayer);
                Player.GetComponent<PlayerMouseMove>().StopPlayerMove();


                ClickedMouse = false;
                ObjectIsOpen = true;
                //TriggerObject.SetActive(false);
                Player.GetComponent<Player>().EnterSomething();
                ObjectAnim = true;
                //StartCoroutine(WaitAnimTable(Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation + 0.1f));
                GetComponent<BoxCollider>().enabled = false;
            }
            else if (ArgumentsNotQuit == 0 && !ObjectAnim && ObjectIsOpen && Input.GetMouseButtonDown(1))
            {
                //TriggerObject.SetActive(false);
                //InTrigger = false;
                ObjectIsOpen = false;
                ObjectAnim = true;
                ClickedMouse = false;

                //Vcam.GetComponent<MoveCameraAnimation>().startCoords = currentPosVcam;
                //Vcam.GetComponent<MoveCameraAnimation>().needPosition = true;
                //Vcam.GetComponent<MoveCameraAnimation>().startRotate = currentRotVcam;
                //Vcam.GetComponent<MoveCameraAnimation>().needRotate = true;
                //Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation = TimeAnimationVcam;
                //Vcam.GetComponent<MoveCameraAnimation>().StartMove();

                //StartCoroutine(WaitAnimTable(Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation + 0.1f));
                //StartCoroutine(WaitAnimCamera(Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation + 0.1f));

                Player.GetComponent<PlayerMouseMove>().MovePlayer(currentPosPlayer);
                Player.GetComponent<PlayerMouseMove>().ReturnPlayerMove();

                GetComponent<BoxCollider>().enabled = true;
            }
        }
        IEnumerator WaitAnimTable(float f)
        {
            if (ObjectIsOpen == false)
                DisapperanceAnimationTV?.Invoke();

            yield return new WaitForSeconds(f);
            ObjectAnim = false;

            if (ObjectIsOpen)
                ApperanceAnimationTV?.Invoke();
        }
        IEnumerator WaitAnimCamera(float f)
        {
            yield return new WaitForSeconds(f);
            Player.GetComponent<Player>().PutItem();
        }
    }
}
