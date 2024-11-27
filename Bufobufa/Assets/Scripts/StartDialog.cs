using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialog : MonoBehaviour
{
    [SerializeField] DialogManager Dialog;
    private void Start()
    {
        StartCoroutine(WaitWhat());
    }
    IEnumerator WaitWhat()
    {
        yield return new WaitForSeconds(1);
        Dialog.StartDialog(0);
    }
}
