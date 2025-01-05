using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOpen : MonoBehaviour
{
    private GameObject Player;
    private GameObject MixTable;


    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MixTable = transform.Find("MixTable").gameObject;
    }
    private void Update()
    {
        if (GetComponent<OpenObject>().ObjectIsOpen && !MixTable.GetComponent<ThingsInTableMix>().MixTableOn)
        {
            MixTable.GetComponent<ThingsInTableMix>().MixTableOn = true;
        }
        else if (!GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().ObjectIsOpen && Input.GetMouseButtonDown(1))
        {
            MixTable.GetComponent<ThingsInTableMix>().MixTableOn = false;
            if (MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject != null)
            {
                Player.GetComponent<PlayerInfo>().currentPickObject = MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject;
                MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject.transform.parent = Player.transform;
                MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject = null;
                Player.GetComponent<PlayerInfo>().playerPickSometing = true;
            }
        }
    }
}
