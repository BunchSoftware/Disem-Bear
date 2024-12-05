using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TargetModelOpen : MonoBehaviour
{
    private ModelBoard board;
    private bool OneTap = true;

    public List<DialogTarget> targets = new();

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
        board = GetComponent<ModelBoard>();
        DialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
    }
    private void Update()
    {
        if (board.ModelOpen && OneTap)
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
        else if (!board.ModelOpen && !OneTap)
        {
            OneTap = true;
        }
    }
}
