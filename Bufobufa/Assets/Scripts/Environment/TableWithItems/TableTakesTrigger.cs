using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTakesTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<TableWithItems>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<TableWithItems>().OnTrigExit(other);
    }
}
