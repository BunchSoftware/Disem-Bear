using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<TVOpen>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<TVOpen>().OnTrigExit(other);
    }
}
