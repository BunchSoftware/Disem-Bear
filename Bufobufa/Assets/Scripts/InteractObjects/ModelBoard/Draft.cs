using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draft : MonoBehaviour
{
    private bool InText = false;
    private bool WhileAnimGo = false;
    private float OrigXSizeColliser;
    [SerializeField] DialogManager Dialog; //”ƒ¿À»“‹
    public string CodeWord = ""; //”ƒ¿À»“‹
    [SerializeField] GameObject OffStrela; //”ƒ¿À»“‹
    [SerializeField] GameObject OnStrela; //”ƒ¿À»“‹
    private void Start()
    {
        Dialog = GameObject.Find("DialogManager").GetComponent<DialogManager>(); //”ƒ¿À»“‹
        OffStrela = GameObject.Find("Strelki").transform.Find("Strelka (2)").gameObject; //”ƒ¿À»“‹
        OnStrela = GameObject.Find("Strelki").transform.Find("Strelka (3)").gameObject; //”ƒ¿À»“‹

    }
    private void OnMouseDown()
    {
        if (!WhileAnimGo && !transform.parent.GetComponent<OrgansBoardOpen>().OpenModel)
        {
            transform.parent.GetComponent<OrgansBoardOpen>().OpenModel = true;

            Dialog.RunConditionSkip(CodeWord); //”ƒ¿À»“‹
            OffStrela.SetActive(false); //”ƒ¿À»“‹
            OnStrela.SetActive(true); //”ƒ¿À»“‹

            OrigXSizeColliser = GetComponent<BoxCollider>().size.x;
            GetComponent<BoxCollider>().size = new Vector3(1f, GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z);
            GetComponent<MoveAnimation>().StartMove();
            InText = true;
            WhileAnimGo = true;
            StartCoroutine(WaitAnimGo(GetComponent<MoveAnimation>().TimeAnimation));
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && InText && !WhileAnimGo && transform.parent.GetComponent<OrgansBoardOpen>().OpenModel)
        {
            transform.parent.GetComponent<OrgansBoardOpen>().OpenModel = false;
            GetComponent<BoxCollider>().size = new Vector3(OrigXSizeColliser, GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z);
            GetComponent<MoveAnimation>().EndMove();
            InText = false;
            WhileAnimGo = true;
            StartCoroutine(WaitAnimGo(GetComponent<MoveAnimation>().TimeAnimation));
        }
    }
    IEnumerator WaitAnimGo(float t)
    {
        yield return new WaitForSeconds(t);
        WhileAnimGo = false;
    }
}
