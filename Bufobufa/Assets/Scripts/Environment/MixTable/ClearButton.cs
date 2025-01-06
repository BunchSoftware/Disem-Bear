using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Environment.MixTable
{
    public class ClearButton : MonoBehaviour
    {
        private GameObject MixTable;

        private void Start()
        {
            MixTable = GameObject.Find("MixTable");
        }

        private void OnMouseDown()
        {
            MixTable.GetComponent<ThingsInTableMix>().ClearIngredients();
            transform.parent.gameObject.GetComponent<Animator>().Play("PressButtonClear");
        }
    }
}
