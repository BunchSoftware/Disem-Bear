using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<Printer>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<Printer>().OnTrigExit(other);
    }
}
