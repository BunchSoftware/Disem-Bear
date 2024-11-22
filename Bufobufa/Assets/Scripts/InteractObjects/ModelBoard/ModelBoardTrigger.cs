using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBoardTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.GetComponent<ModelBoardOpen>().OnTrigEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        transform.parent.gameObject.GetComponent<ModelBoardOpen>().OnTrigExit(other);
    }
}
