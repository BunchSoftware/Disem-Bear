using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.LPostTube;
using Game.LDialog;
using Unity.VisualScripting;
using UnityEngine;

public class SetActiveTabsInTVTutorial : MonoBehaviour
{
    private DialogManager dialogManager;
    [SerializeField] private GameObject OrdersButton;
    [SerializeField] private GameObject ShopButton;
    [SerializeField] private GameObject ChatButton;
    [SerializeField] private GameObject ResourceButton;

    [SerializeField] private List<SetActiveTabs> setActiveTabs = new();



    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;
        this.dialogManager.OnStartDialog.AddListener(SetActiveTVTabsStartDialog);
        this.dialogManager.OnFullEndDialog.AddListener(SetActiveTVTabsFullEndDialog);
    }

    public void SetActiveTVTabsStartDialog(Dialog dialog)
    {
        for (int i = 0; i < setActiveTabs.Count; i++)
        {
            if (dialogManager.GetCurrentIndexDialogPoint() == setActiveTabs[i].indexDialogPoint && dialogManager.GetCurrentIndexDialog() == 
                setActiveTabs[i].indexDialog && setActiveTabs[i].needTargetFullEndDialog == false)
            {
                OrdersButton.SetActive(setActiveTabs[i].ordersButton);
                ShopButton.SetActive(setActiveTabs[i].shopButton);
                ChatButton.SetActive(setActiveTabs[i].chatButton);
                ResourceButton.SetActive(setActiveTabs[i].resourcesButton);
            }
        }
    }

    public void SetActiveTVTabsFullEndDialog(Dialog dialog)
    {
        for (int i = 0; i < setActiveTabs.Count; i++)
        {
            if (dialogManager.GetCurrentIndexDialogPoint() == setActiveTabs[i].indexDialogPoint && dialogManager.GetCurrentIndexDialog() ==
                setActiveTabs[i].indexDialog && setActiveTabs[i].needTargetFullEndDialog)
            {
                OrdersButton.SetActive(setActiveTabs[i].ordersButton);
                ShopButton.SetActive(setActiveTabs[i].shopButton);
                ChatButton.SetActive(setActiveTabs[i].chatButton);
                ResourceButton.SetActive(setActiveTabs[i].resourcesButton);
            }
        }
    }


    [Serializable]
    public class SetActiveTabs
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;

        public bool needTargetFullEndDialog = false;

        public bool ordersButton = true;
        public bool shopButton = true;
        public bool chatButton = true;
        public bool resourcesButton = true;
    }
}
