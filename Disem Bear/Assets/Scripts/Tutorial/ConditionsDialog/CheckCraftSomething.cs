using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.LDialog;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckCraftSomething : MonoBehaviour
{
    public enum TypeCraftObject
    {
        Ingradient,
        PickUpItem
    }

    [SerializeField] private List<CraftSomethingCondition> conditions = new();
    [SerializeField] private List<CraftSomethingStartDialog> startDialogs = new();

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
            if (dialogManager.GetCurrentIndexDialogPoint() == conditions[i].indexDialogPoint && dialogManager.GetCurrentIndexDialog() == conditions[i].indexDialog && 
                conditions[i].craftObject == TypeCraftObject.Ingradient && conditions[i].name == ingradient.typeIngradient)
            {
                dialogManager.SkipReplica();
                break;
            }
        }
        for (int i = 0; i < startDialogs.Count; i++)
        {
            if (startDialogs[i].craftObject == TypeCraftObject.Ingradient && startDialogs[i].name == ingradient.typeIngradient)
            {
                dialogManager.StartDialog(startDialogs[i].indexDialog);
                startDialogs.Remove(startDialogs[i]);
                break;
            }
        }
    }

    public void CheckPickUpItem(PickUpItem pickUpItem)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (dialogManager.GetCurrentIndexDialogPoint() == conditions[i].indexDialogPoint && dialogManager.GetCurrentIndexDialog() == conditions[i].indexDialog && 
                conditions[i].craftObject == TypeCraftObject.PickUpItem && conditions[i].name == pickUpItem.NameItem)
            {
                dialogManager.SkipReplica();
                break;
            }
        }
        for (int i = 0; i < startDialogs.Count; i++)
        {
            if (startDialogs[i].craftObject == TypeCraftObject.PickUpItem && startDialogs[i].name == pickUpItem.NameItem)
            {
                dialogManager.StartDialog(startDialogs[i].indexDialog);
                startDialogs.Remove(startDialogs[i]);
                break;
            }
        }
    }

    [Serializable]
    public class CraftSomethingCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public TypeCraftObject craftObject;
        public string name;
    }

    [Serializable]
    public class CraftSomethingStartDialog
    {
        public TypeCraftObject craftObject;
        public string name;
        [Space]
        public int indexDialog = 0;
    }
}
