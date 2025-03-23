using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.Item;
using Game.LDialog;
using UnityEngine;
using static CheckCraftSomething;

public class CheckOpenSomething : MonoBehaviour
{
    public enum WhatTrack
    {
        StartOpen,
        EndOpen,
        StartClose,
        EndClose
    }

    [SerializeField] private List<OpenSomethingCondition> conditions = new();
    [SerializeField] private List<OpenSomethingStartDialog> startDialogs = new();

    private DialogManager dialogManager;

    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;
        
        for (int i = 0; i < conditions.Count; i++)
        {
            OpenSomethingCondition condition = conditions[i];
            switch (condition.whatTrack)
            {
                case WhatTrack.StartOpen:
                    condition.openObject.OnStartObjectOpen.AddListener(() =>
                    {
                        if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog)
                        {
                            dialogManager.SkipReplica();
                        }
                    });
                    break;
                case WhatTrack.EndOpen:
                    condition.openObject.OnEndObjectOpen.AddListener(() =>
                    {
                        if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog)
                        {
                            dialogManager.SkipReplica();
                        }
                    });
                    break;
                case WhatTrack.StartClose:
                    condition.openObject.OnStartObjectClose.AddListener(() =>
                    {
                        if (dialogManager.IsDialogOn() && dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog)
                        {
                            dialogManager.SkipReplica();
                        }
                    });
                    break;
                case WhatTrack.EndClose:
                    condition.openObject.OnEndObjectClose.AddListener(() =>
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
            OpenSomethingStartDialog startDialog = startDialogs[i];
            switch (startDialog.whatTrack)
            {
                case WhatTrack.StartOpen:
                    startDialog.openObject.OnStartObjectOpen.AddListener((UnityEngine.Events.UnityAction)(() =>
                    {
                        if (dialogManager.IsDialogOn() == false && startDialog.on)
                        {
                            dialogManager.StartDialog(startDialog.indexDialogPoint);
                            startDialog.on = false;
                        }
                    }));
                    break;
                case WhatTrack.EndOpen:
                    startDialog.openObject.OnEndObjectOpen.AddListener((UnityEngine.Events.UnityAction)(() =>
                    {
                        if (dialogManager.IsDialogOn() == false && startDialog.on)
                        {
                            dialogManager.StartDialog(startDialog.indexDialogPoint);
                            startDialog.on = false;
                        }
                    }));
                    break;
                case WhatTrack.StartClose:
                    startDialog.openObject.OnStartObjectClose.AddListener((UnityEngine.Events.UnityAction)(() =>
                    {
                        if (dialogManager.IsDialogOn() == false && startDialog.on)
                        {
                            dialogManager.StartDialog(startDialog.indexDialogPoint);
                            startDialog.on = false;
                        }
                    }));
                    break;
                case WhatTrack.EndClose:
                    startDialog.openObject.OnEndObjectClose.AddListener((UnityEngine.Events.UnityAction)(() =>
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
    public class OpenSomethingCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public OpenObject openObject;
        public WhatTrack whatTrack;
    }


    [Serializable]
    public class OpenSomethingStartDialog
    {
        public OpenObject openObject;
        public WhatTrack whatTrack;
        public int indexDialogPoint = 0;
        public bool on = false;
    }
}
