using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrgansBoardTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<OrgansBoardOpen>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<OrgansBoardOpen>().OnTrigExit(other);
    }
}
