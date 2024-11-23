using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPackage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<GetItemFromTable>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<GetItemFromTable>().OnTrigExit(other);
    }
}
