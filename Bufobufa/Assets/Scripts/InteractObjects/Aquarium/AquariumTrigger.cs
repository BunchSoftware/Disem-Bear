using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquariumTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<AquariumOpen>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<AquariumOpen>().OnTrigExit(other);
    }
}
