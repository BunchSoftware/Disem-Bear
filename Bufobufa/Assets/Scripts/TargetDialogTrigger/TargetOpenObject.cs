using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOpenObject : MonoBehaviour
{
    public List<DialogTarget> targets = new();

    private bool OneTap = true;

    private OpenObject OpenObj;
    private DialogManager DialogManager;

    [System.Serializable]
    public class DialogTarget
    {
        public bool Active = false;
        public string DialogTag = "";
        public bool NewDialog = false;
        public int UniqId = '1';
    }

    private void Start()
    {
        OpenObj = GetComponent<OpenObject>();
        DialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
    }

    private void Update()
    {
        if (OpenObj.ObjectIsOpen && OneTap)
        {
            OneTap = false;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].Active)
                {
                    if (targets[i].NewDialog)
                    {
                        //DialogManager.StartDialog(targets[i].DialogTag)
                    }
                }
            }
        }
        else if (!OpenObj.ObjectIsOpen &&  OneTap == false)
        {
            OneTap = true;
        }
    }
}
