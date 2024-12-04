using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTrigger : MonoBehaviour
{
    public int NumPoint = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (transform.parent.GetComponent<TableTakesItem>().points[NumPoint].GetItem)
        {
            transform.parent.GetComponent<TableTakesItem>().points[NumPoint].obj.GetComponent<GetItemFromTable>().OnTrigEnter(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (transform.parent.GetComponent<TableTakesItem>().points[NumPoint].GetItem)
        {
            transform.parent.GetComponent<TableTakesItem>().points[NumPoint].obj.GetComponent<GetItemFromTable>().OnTrigExit(other);
        }
    }
}
