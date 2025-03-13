using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.LDialog;
using UnityEngine;

public class CraftSomethingConditions : MonoBehaviour
{
    public enum TypeCraftObject
    {
        Ingradient,
        PickUpItem
    }

    [SerializeField] List<CraftSomethingCondition> conditions = new();

    private DialogManager dialogManager;
    private Workbench workbench;

    public void Init(DialogManager dialogManager, Workbench workbench)
    {
        this.dialogManager = dialogManager;
        this.workbench = workbench;

        workbench.OnCreatIngradient.AddListener(CheckIngradient);
        workbench.OnCreatPickUpItem.AddListener(CheckPickUpItem);
    }

    public void CheckIngradient(IngradientData ingradient)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].craftObject == TypeCraftObject.Ingradient && conditions[i].name == ingradient.typeIngradient)
            {
                dialogManager.RunConditionSkip(conditions[i].condition);
                break;
            }
        }
    }

    public void CheckPickUpItem(PickUpItem pickUpItem)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].craftObject == TypeCraftObject.PickUpItem && conditions[i].name == pickUpItem.NameItem)
            {
                dialogManager.RunConditionSkip(conditions[i].condition);
                break;
            }
        }
    }

    [Serializable]
    public class CraftSomethingCondition
    {
        public TypeCraftObject craftObject;
        public string name;
        [Space]
        public string condition = "None";
    }
}
