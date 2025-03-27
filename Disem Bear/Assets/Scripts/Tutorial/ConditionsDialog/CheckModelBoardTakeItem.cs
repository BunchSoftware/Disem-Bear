using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.LModelBoard;
using Game.LDialog;
using UnityEngine;

public class CheckModelBoardTakeItem : MonoBehaviour
{
    [SerializeField] private List<ModelBoardTakeItemCondition> conditions = new();
    [SerializeField] private List<ModelBoardTakeItemStartDialog> startDialogs = new();

    private DialogManager dialogManager;

    public void Init(DialogManager dialogManager, CellModelBoard cellModelBoard1, CellModelBoard cellModelBoard2, CellModelBoard cellModelBoard3, CellModelBoard cellModelBoard4)
    {
        this.dialogManager = dialogManager;

        cellModelBoard1.OnPutItem.AddListener((pickUpItem) =>
        {
            Debug.Log(pickUpItem.NameItem);
            for (int k = 0; k < conditions.Count; k++)
            {
                if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == conditions[k].indexDialogPoint
                && dialogManager.GetCurrentIndexDialog() == conditions[k].indexDialog && pickUpItem.NameItem == conditions[k].nameItem)
                {
                    dialogManager.SkipReplica();
                    break;
                }
            }
            for (int k = 0; k < startDialogs.Count; k++)
            {
                if (dialogManager.IsDialogOn() == false && startDialogs[k].on && pickUpItem.NameItem == startDialogs[k].nameItem)
                {
                    dialogManager.StartDialog(startDialogs[k].indexDialogPoint);
                    startDialogs[k].on = false;
                    break;
                }
            }
        });
        cellModelBoard2.OnPutItem.AddListener((pickUpItem) =>
        {
            Debug.Log(pickUpItem.NameItem);
            for (int k = 0; k < conditions.Count; k++)
            {
                if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == conditions[k].indexDialogPoint
                && dialogManager.GetCurrentIndexDialog() == conditions[k].indexDialog && pickUpItem.NameItem == conditions[k].nameItem)
                {
                    dialogManager.SkipReplica();
                    break;
                }
            }
            for (int k = 0; k < startDialogs.Count; k++)
            {
                if (dialogManager.IsDialogOn() == false && startDialogs[k].on && pickUpItem.NameItem == startDialogs[k].nameItem)
                {
                    dialogManager.StartDialog(startDialogs[k].indexDialogPoint);
                    startDialogs[k].on = false;
                    break;
                }
            }
        });
        cellModelBoard3.OnPutItem.AddListener((pickUpItem) =>
        {
            Debug.Log(pickUpItem.NameItem);
            for (int k = 0; k < conditions.Count; k++)
            {
                if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == conditions[k].indexDialogPoint
                && dialogManager.GetCurrentIndexDialog() == conditions[k].indexDialog && pickUpItem.NameItem == conditions[k].nameItem)
                {
                    dialogManager.SkipReplica();
                    break;
                }
            }
            for (int k = 0; k < startDialogs.Count; k++)
            {
                if (dialogManager.IsDialogOn() == false && startDialogs[k].on && pickUpItem.NameItem == startDialogs[k].nameItem)
                {
                    dialogManager.StartDialog(startDialogs[k].indexDialogPoint);
                    startDialogs[k].on = false;
                    break;
                }
            }
        });
        cellModelBoard4.OnPutItem.AddListener((pickUpItem) =>
        {
            Debug.Log(pickUpItem.NameItem);
            for (int k = 0; k < conditions.Count; k++)
            {
                if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == conditions[k].indexDialogPoint
                && dialogManager.GetCurrentIndexDialog() == conditions[k].indexDialog && pickUpItem.NameItem == conditions[k].nameItem)
                {
                    dialogManager.SkipReplica();
                    break;
                }
            }
            for (int k = 0; k < startDialogs.Count; k++)
            {
                if (dialogManager.IsDialogOn() == false && startDialogs[k].on && pickUpItem.NameItem == startDialogs[k].nameItem)
                {
                    dialogManager.StartDialog(startDialogs[k].indexDialogPoint);
                    startDialogs[k].on = false;
                    break;
                }
            }
        });
    }


    [Serializable]
    public class ModelBoardTakeItemCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public string nameItem;
    }


    [Serializable]
    public class ModelBoardTakeItemStartDialog
    {
        public string nameItem;
        public int indexDialogPoint = 0;
        public bool on = false;
    }
}
