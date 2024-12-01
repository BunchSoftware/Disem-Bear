using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draft : MonoBehaviour
{
    private bool InText = false;
    private bool WhileAnimGo = false;

    private void OnMouseDown()
    {
        if (!WhileAnimGo && transform.parent.GetComponent<OpenObject>().ObjectIsOpen && !transform.parent.GetComponent<OrgansBoardOpen>().ModelOpen)
        {
            transform.parent.GetComponent<OrgansBoardOpen>().ModelOpen = true;
            transform.parent.GetComponent<OpenObject>().ArgumentsNotQuit += 1;

            GetComponent<MouseTrigger>().AnyCase = false;

            GetComponent<MoveAnimation>().StartMove();
            InText = true;
            WhileAnimGo = true;
            StartCoroutine(WaitAnimGo(GetComponent<MoveAnimation>().TimeAnimation));
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && InText && !WhileAnimGo && transform.parent.GetComponent<OpenObject>().ObjectIsOpen)
        {
            transform.parent.GetComponent<OrgansBoardOpen>().ModelOpen = false;
            transform.parent.GetComponent<OpenObject>().ArgumentsNotQuit -= 1;

            GetComponent<MouseTrigger>().AnyCase = true;

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
