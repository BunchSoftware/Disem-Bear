using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayerPickSomething : MonoBehaviour
{
    public List<DialogTarget> targets = new();

    private bool OneTap = true;

    private PlayerInfo Player_Info;
    private DialogManager DialogManager;

    [System.Serializable]
    public class DialogTarget
    {
        public bool Active = false;
        public bool StayActiveAfter = false;
        public string DialogTag = "";
        public bool NewDialog = false;
        public int NumDialog = 0;
        public int UniqId = '1';
        public List<ActivateObjects> NeedActivate = new();
    }
    [System.Serializable]
    public class ActivateObjects
    {
        public GameObject obj;
        public List<int> Ids = new();
    }

    private void Start()
    {
        Player_Info = GetComponent<PlayerInfo>();
        DialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
    }

    private void Update()
    {
        if (Player_Info.playerPickSometing && Player_Info.currentPickObject.GetComponent<PackageInfo>() && OneTap)
        {
            OneTap = false;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].Active)
                {
                    if (targets[i].NewDialog)
                    {
                        DialogManager.StartDialog(targets[i].NumDialog);
                    }
                    else
                    {
                        DialogManager.RunConditionSkip(targets[i].DialogTag);
                    }
                    if (!targets[i].StayActiveAfter)
                    {
                        targets[i].Active = false;
                    }
                    break;
                }
            }
        }
        else if (!Player_Info.playerPickSometing && OneTap == false)
        {
            OneTap = true;
        }
    }
}
