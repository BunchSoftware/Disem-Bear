using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.LMixTable;
using Game.LDialog;
using UnityEngine;

public class CheckButtonsWorkbench : MonoBehaviour
{
    public enum TypeButton
    {
        PickUpButton,
        MixButton,
        ClearButton
    }
    [SerializeField] private List<PressButtonCondition> conditions = new();
    [SerializeField] private List<PressButtonStartDialog> startDialogs = new();

    private PickUpButton pickUpButton;
    private MixButton mixButton;
    private ClearButton clearButton;

    public void Init(PickUpButton pickUpButton, MixButton mixButton, ClearButton clearButton, DialogManager dialogManager)
    {
        this.pickUpButton = pickUpButton;
        this.mixButton = mixButton;
        this.clearButton = clearButton;

        for (int i = 0; i < conditions.Count; i++)
        {
            PressButtonCondition condition = conditions[i];
            switch (condition.typeButton)
            {
                case TypeButton.PickUpButton:
                    pickUpButton.pressButton.AddListener(() =>
                    {
                        if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog)
                        {
                            dialogManager.SkipReplica();
                        }
                    });
                    break;
                case TypeButton.MixButton:
                    mixButton.pressButton.AddListener(() =>
                    {
                        if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog)
                        {
                            dialogManager.SkipReplica();
                        }
                    });
                    break;
                case TypeButton.ClearButton:
                    clearButton.pressButton.AddListener(() =>
                    {
                        if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog)
                        {
                            dialogManager.SkipReplica();
                        }
                    });
                    break;
            }
        }
        for (int i = 0; i < startDialogs.Count; i++)
        {
            PressButtonStartDialog startDialog = startDialogs[i];
            switch (startDialog.typeButton)
            {
                case TypeButton.PickUpButton:
                    pickUpButton.pressButton.AddListener((UnityEngine.Events.UnityAction)(() =>
                    {
                        if (dialogManager.IsDialogOn() == false && startDialog.on)
                        {
                            dialogManager.StartDialog(startDialog.indexDialogPoint);
                            startDialog.on = false;
                        }
                    }));
                    break;
                case TypeButton.MixButton:
                    mixButton.pressButton.AddListener((UnityEngine.Events.UnityAction)(() =>
                    {
                        if (dialogManager.IsDialogOn() == false && startDialog.on)
                        {
                            dialogManager.StartDialog(startDialog.indexDialogPoint);
                            startDialog.on = false;
                        }
                    }));
                    break;
                case TypeButton.ClearButton:
                    clearButton.pressButton.AddListener((UnityEngine.Events.UnityAction)(() =>
                    {
                        if (dialogManager.IsDialogOn() == false && startDialog.on)
                        {
                            dialogManager.StartDialog(startDialog.indexDialogPoint);
                            startDialog.on = false;
                        }
                    }));
                    break;
            }
        }
    }


    [Serializable]
    public class PressButtonCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public TypeButton typeButton;
    }

    [Serializable]
    public class PressButtonStartDialog
    {
        public TypeButton typeButton;
        public int indexDialogPoint = 0;
        public bool on = false;
    }
}
