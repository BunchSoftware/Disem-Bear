using System;
using System.Collections;
using System.Collections.Generic;
using Game.LDialog;
using UnityEngine;

public class CheckPlaceSomething : MonoBehaviour
{
    [SerializeField] private List<PlaceSomethingCondition> conditions = new();
    [SerializeField] private List<PlaceSomethingStartDialog> startDialogs = new();

    private DialogManager dialogManager;

    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;

        for (int i = 0; i < conditions.Count; i++)
        {
            PlaceSomethingCondition condition = conditions[i];
            condition.placeItem.OnPlaceItem.AddListener((nameItem) =>
            {
                if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog)
                {
                    dialogManager.SkipReplica();
                }
            });
        }
        for (int i = 0; i < startDialogs.Count; i++)
        {
            PlaceSomethingStartDialog startDialog = startDialogs[i];
            startDialog.placeItem.OnPlaceItem.AddListener((UnityEngine.Events.UnityAction<string>)((nameItem) =>
            {
                if (dialogManager.IsDialogOn() == false && startDialog.on)
                {
                    dialogManager.StartDialog(startDialog.indexDialogPoint);
                    startDialog.on = false;
                }
            }));
        }
    }


    [Serializable]
    public class PlaceSomethingCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public PlaceItem placeItem;
    }


    [Serializable]
    public class PlaceSomethingStartDialog
    {
        public PlaceItem placeItem;
        public int indexDialogPoint = 0;
        public bool on = false;
    }
}
