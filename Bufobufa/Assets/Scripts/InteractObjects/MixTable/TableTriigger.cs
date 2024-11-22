using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTriigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<TableOpen>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<TableOpen>().OnTrigExit(other);
    }
}
