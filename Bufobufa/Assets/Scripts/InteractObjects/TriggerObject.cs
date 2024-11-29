using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<OpenObject>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<OpenObject>().OnTrigExit(other);
    }
}
