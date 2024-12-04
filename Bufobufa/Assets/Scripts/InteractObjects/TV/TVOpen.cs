using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TVOpen : MonoBehaviour
{
    public UnityEvent ApperanceAnimationTV;
    public UnityEvent DisapperanceAnimationTV;
    private bool IsTrigger = false;
    private bool TVIsOpen = false;
    private bool TvIsAnimation = false;
    private bool IsClickedMouse = false;

    private int argumentsNotQuit = 0;

    private PlayerMouseMove player;
    private MoveCameraAnimation virtualCamera;
    private GameObject triggerTv;
    private GameObject mainCamera;


    [Header("Координаты куда должен уйти объект при открытии стола(Игрок и камера)")]
    public Vector3 positionPlayer = new();
    public float timeAnimationPlayer = 1f;
    public Vector3 positionVirtualCamera = new();
    public Quaternion rotationVirtualCamera = new();
    public float timeAnimationVirtualCamera = 1f;

    private Vector3 currentPositionPlayer = new();
    private Vector3 currentPositionVirtualCamera = new();
    private Quaternion currentRotationVirtualCamera = new();


    private void Start()
    {
        virtualCamera = GameObject.FindGameObjectWithTag("Vcam").GetComponent<MoveCameraAnimation>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMouseMove>();
        triggerTv = transform.Find("TriggerObject").gameObject;
    }
    public void OnTrigEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            IsTrigger = true;
        }
    }
    public void OnTrigExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsTrigger = false;
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("Floor", "ClickedObject")))
            {
                if (infoHit.collider.gameObject == gameObject && !TVIsOpen)
                {
                    IsClickedMouse = true;
                    triggerTv.SetActive(true);
                }
                else
                {
                    IsClickedMouse = false;
                    triggerTv.SetActive(false);
                }
            }
        }


        if (!player.GetComponent<PlayerInfo>().PlayerPickSometing && !TvIsAnimation && IsTrigger && IsClickedMouse && !TVIsOpen)
        {
            var tmpPosCamera = mainCamera.transform.position;
            virtualCamera.transform.position = tmpPosCamera;
            currentPositionVirtualCamera = tmpPosCamera;
            currentRotationVirtualCamera = virtualCamera.transform.rotation;

            virtualCamera.startCoords = positionVirtualCamera;
            virtualCamera.needPosition = true;
            virtualCamera.startRotate = rotationVirtualCamera;
            virtualCamera.needRotate = true;
            virtualCamera.TimeAnimation = timeAnimationVirtualCamera;
            virtualCamera.StartMove();


            currentPositionPlayer = player.transform.position;
            player.MovePlayer(positionPlayer);
            player.StopPlayerMove();


            IsClickedMouse = false;
            TVIsOpen = true;
            triggerTv.SetActive(false);
            player.GetComponent<PlayerInfo>().PlayerInSomething = true;
            TvIsAnimation = true;
            StartCoroutine(WaitAnimTable(virtualCamera.TimeAnimation + 0.1f));
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (argumentsNotQuit == 0 && !TvIsAnimation && TVIsOpen && Input.GetMouseButtonDown(1))
        {
            triggerTv.SetActive(false);
            IsTrigger = false;
            TVIsOpen = false;
            TvIsAnimation = true;
            IsClickedMouse = false;

            virtualCamera.startCoords = currentPositionVirtualCamera;
            virtualCamera.needPosition = true;
            virtualCamera.startRotate = currentRotationVirtualCamera;
            virtualCamera.needRotate = true;
            virtualCamera.TimeAnimation = timeAnimationVirtualCamera;
            virtualCamera.StartMove();

            StartCoroutine(WaitAnimTable(virtualCamera.TimeAnimation + 0.1f));
            StartCoroutine(WaitAnimCamera(virtualCamera.TimeAnimation + 0.1f));

            player.MovePlayer(currentPositionPlayer);
            player.ReturnPlayerMove();

            GetComponent<BoxCollider>().enabled = true;
        }
    }
    IEnumerator WaitAnimTable(float f)
    {
        if(TVIsOpen == false)
            DisapperanceAnimationTV?.Invoke();

        yield return new WaitForSeconds(f);
        TvIsAnimation = false;

        if (TVIsOpen)
            ApperanceAnimationTV?.Invoke();
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        player.GetComponent<PlayerInfo>().PlayerInSomething = false;
    }

    public bool GetTVIsOpen()
    {
        return TVIsOpen;
    }
}
