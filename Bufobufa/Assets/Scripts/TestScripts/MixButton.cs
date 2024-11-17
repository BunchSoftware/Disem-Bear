using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixButton : MonoBehaviour
{
    private GameObject MixTable;

    private void Start()
    {
        MixTable = GameObject.Find("MixTable");
    }

    private void OnMouseDown()
    {
        MixTable.GetComponent<ThingsInTableMix>().MixIngredients();
        transform.parent.gameObject.GetComponent<Animator>().Play("ButtonPress");
    }
}
