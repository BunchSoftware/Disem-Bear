using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPointerManager : MonoBehaviour
{
    public void SetPointer(int NumPointer)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
        transform.GetChild(NumPointer).GetChild(0).gameObject.SetActive(true);
    }
}
