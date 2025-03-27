using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.Aquarium;
using Game.LDialog;
using UnityEngine;

public class CheckAquariumDirty : MonoBehaviour
{
    [SerializeField] private List<CheckAquariumDirtyCondition> conditions = new();
    [SerializeField] private List<CheckAquariumDirtyStartDialog> startDialogs = new();


    public void Init(DialogManager dialogManager)
    {

        for (int i = 0; i < conditions.Count; i++)
        {
            CheckAquariumDirtyCondition condition = conditions[i];
            conditions[i].aquarium.OnAquariumBecomeDirty.AddListener(() =>
            {
                if (dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog)
                {
                    dialogManager.SkipReplica();
                }
            });
        }
        for (int i = 0; i < startDialogs.Count; i++)
        {
            CheckAquariumDirtyStartDialog startDialog = startDialogs[i];
            startDialog.aquarium.OnAquariumBecomeDirty.AddListener(() =>
            {
                if (startDialog.on)
                {
                    dialogManager.StartDialog(startDialog.indexDialogPoint);
                    startDialog.on = false;
                }
            });
        }
    }


    [Serializable]
    public class CheckAquariumDirtyCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public Aquarium aquarium;
    }

    [Serializable]
    public class CheckAquariumDirtyStartDialog
    {
        public Aquarium aquarium;
        public int indexDialogPoint = 0;
        public bool on = false;
    }
}
