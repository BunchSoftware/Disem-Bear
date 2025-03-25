using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.Aquarium;
using Game.LDialog;
using Game.LPlayer;
using UnityEngine;

public class OnAquariumTutorialManager : MonoBehaviour
{
    private DialogManager dialogManager;

    [SerializeField] private List<OnAquarium> onAquariums = new();

    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;
        this.dialogManager.OnStartDialog.AddListener(OnAquariumStartDialog);
        this.dialogManager.OnFullEndDialog.AddListener(OnAquariumFullEndDialog);

    }

    public void OnAquariumStartDialog(Dialog dialog)
    {
        for (int i = 0; i < onAquariums.Count; i++)
        {
            if (onAquariums[i].indexDialogPoint == dialogManager.GetCurrentIndexDialogPoint() && onAquariums[i].indexDialog == dialogManager.GetCurrentIndexDialog())
            {
                if (onAquariums[i].needTargetFullEndDialog == false)
                {
                    onAquariums[i].aquarium.on = onAquariums[i].AquariumOn;
                    return;
                }
            }
        }
    }

    public void OnAquariumFullEndDialog(Dialog dialog)
    {
        for (int i = 0; i < onAquariums.Count; i++)
        {
            if (onAquariums[i].indexDialogPoint == dialogManager.GetCurrentIndexDialogPoint() && onAquariums[i].indexDialog == dialogManager.GetCurrentIndexDialog())
            {
                if (onAquariums[i].needTargetFullEndDialog)
                {
                    onAquariums[i].aquarium.on = onAquariums[i].AquariumOn;
                    return;
                }
            }
        }
    }

    [Serializable]
    public class OnAquarium
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        [Space]
        public bool needTargetFullEndDialog = false;
        [Space]
        [Space]
        public bool AquariumOn = false;
        [Space]
        public Aquarium aquarium;
    }
}
