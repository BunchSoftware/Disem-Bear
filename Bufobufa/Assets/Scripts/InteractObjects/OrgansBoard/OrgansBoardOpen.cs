using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrgansBoardOpen : MonoBehaviour
{
    public bool InTrigger = false;
    public bool OrgansBoardIsOpen = false;
    private bool OrgansBoardAnim = false;
    private bool ClickedMouse = false;
    public bool OpenModel = false;

    public List<GameObject> points = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();

    private GameObject Player;
    private GameObject Vcam;
    private GameObject TriggerOrgansBoard;

    [Header(" ÓÓ‰ËÌ‡Ú˚ ÍÛ‰‡ ‰ÓÎÊÂÌ ÛÈÚË Ó·˙ÂÍÚ ÔË ÓÚÍ˚ÚËË ÒÚÓÎ‡(»„ÓÍ Ë Í‡ÏÂ‡)")]
    public Vector3 CoordPlayer = new();
    public float TimeAnimationPlayer = 1f;
    public Vector3 CoordVcam = new();
    public Quaternion RotateVcam = new();
    public float TimeAnimationVcam = 1f;

    private Vector3 currentPos = new();

    [SerializeField] DialogManager Dialog; //”ƒ¿À»“‹
    public string CodeWord = ""; //”ƒ¿À»“‹
    [SerializeField] GameObject OffStrela; //”ƒ¿À»“‹
    [SerializeField] GameObject OnStrela; //”ƒ¿À»“‹


    private void Start()
    {
        Dialog = GameObject.Find("DialogManager").GetComponent<DialogManager>(); //”ƒ¿À»“‹

        Vcam = GameObject.FindGameObjectWithTag("Vcam");
        Player = GameObject.FindGameObjectWithTag("Player");
        TriggerOrgansBoard = transform.Find("TriggerOrgansBoard").gameObject;
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
                    TriggerOrgansBoard.SetActive(true);
                }
                else
                {
                    ClickedMouse = false;
                    TriggerOrgansBoard.SetActive(false);
                }
            }
        }


        if (!OpenModel && !Player.GetComponent<PlayerInfo>().PlayerPickSometing && !OrgansBoardAnim && InTrigger && ClickedMouse && !OrgansBoardIsOpen && !Player.GetComponent<PlayerInfo>().PlayerInSomething)
        {

            ClickedMouse = false;
            Vcam.GetComponent<CinemachineVirtualCamera>().Follow = null;
            Vcam.GetComponent<MoveAnimation>().startCoords = CoordVcam;
            Vcam.GetComponent<MoveAnimation>().needPosition = true;
            Vcam.GetComponent<MoveAnimation>().startRotate = RotateVcam;
            Vcam.GetComponent<MoveAnimation>().needRotate = true;
            Vcam.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationVcam;
            Vcam.GetComponent<MoveAnimation>().StartMove();


            currentPos = Player.transform.position;
            Player.GetComponent<PlayerMouseMove>().MovePlayer(CoordPlayer);
            Player.GetComponent<PlayerMouseMove>().StopPlayerMove();


            OrgansBoardIsOpen = true;
            Player.GetComponent<PlayerInfo>().PlayerInSomething = true;
            OrgansBoardAnim = true;
            StartCoroutine(WaitAnimTable(Vcam.GetComponent<MoveAnimation>().TimeAnimation));
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (!OpenModel && !OrgansBoardAnim && OrgansBoardIsOpen && Input.GetMouseButtonDown(1))
        {
            TriggerOrgansBoard.SetActive(false);
            OrgansBoardIsOpen = false;
            Player.GetComponent<PlayerInfo>().PlayerInSomething = false;
            OrgansBoardAnim = true;
            ClickedMouse = false;
            Vcam.GetComponent<MoveAnimation>().EndMove();
            StartCoroutine(WaitAnimTable(Vcam.GetComponent<MoveAnimation>().TimeAnimation));
            StartCoroutine(WaitAnimCamera(Vcam.GetComponent<MoveAnimation>().TimeAnimation));

            Player.GetComponent<PlayerMouseMove>().MovePlayer(currentPos);
            Player.GetComponent<PlayerMouseMove>().ReturnPlayerMove();


            GetComponent<BoxCollider>().enabled = true;
        }
        else if (!OpenModel && InTrigger && ClickedMouse && !OrgansBoardIsOpen && Player.GetComponent<PlayerInfo>().PlayerPickSometing)
        {
            ClickedMouse = false;
            if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>())
            {
                if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().PackageName == "Document")
                {
                    if (items.Count < points.Count)
                    {
                        Dialog.RunConditionSkip(CodeWord); //”ƒ¿À»“‹
                        OffStrela.SetActive(false); //”ƒ¿À»“‹
                        OnStrela.SetActive(true); //”ƒ¿À»“‹

                        GameObject item = Instantiate(Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().ItemInPackage);
                        items.Add(item);
                        items[items.Count - 1].transform.parent = transform;
                        items[items.Count - 1].transform.localPosition = points[items.Count - 1].transform.localPosition;
                        items[items.Count - 1].SetActive(true);
                        Player.GetComponent<PlayerInfo>().PlayerPickSometing = false;
                        Destroy(Player.GetComponent<PlayerInfo>().currentPickObject);
                        Player.GetComponent<PlayerInfo>().currentPickObject = null;
                    }
                }
            }
        }
    }
    IEnumerator WaitAnimTable(float f)
    {
        yield return new WaitForSeconds(f);
        OrgansBoardAnim = false;
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Vcam.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
    }
}
