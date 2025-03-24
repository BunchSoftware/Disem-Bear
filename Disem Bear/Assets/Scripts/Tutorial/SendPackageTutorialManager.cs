using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.LPostTube;
using Game.LDialog;
using UnityEngine;

public class SendPackageTutorialManager : MonoBehaviour
{
    private DialogManager dialogManager;
    [SerializeField] private PostTube postTube;
    [SerializeField] private List<SendPackage> sendPackages = new();


    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;
        this.dialogManager.OnStartDialog.AddListener(SendPackageStartDialog);
        this.dialogManager.OnFullEndDialog.AddListener(SendPackageFullEndDialog);
    }

    public void SendPackageStartDialog(Dialog dialog)
    {
        for (int i = 0; i < sendPackages.Count; i++)
        {
            if (sendPackages[i].indexDialogPoint == dialogManager.GetCurrentIndexDialogPoint() && sendPackages[i].indexDialog == dialogManager.GetCurrentIndexDialog())
            {
                if (sendPackages[i].needTargetFullEndDialog == false)
                {
                    postTube.ObjectFall(sendPackages[i].gameObjectFall);
                    return;
                }
            }
        }
    }

    public void SendPackageFullEndDialog(Dialog dialog)
    {
        for (int i = 0; i < sendPackages.Count; i++)
        {
            if (sendPackages[i].indexDialogPoint == dialogManager.GetCurrentIndexDialogPoint() && sendPackages[i].indexDialog == dialogManager.GetCurrentIndexDialog())
            {
                if (sendPackages[i].needTargetFullEndDialog)
                {
                    postTube.ObjectFall(sendPackages[i].gameObjectFall);
                    return;
                }
            }
        }
    }


    [Serializable]
    public class SendPackage
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        
        public bool needTargetFullEndDialog = false;

        public GameObject gameObjectFall;
    }
}
