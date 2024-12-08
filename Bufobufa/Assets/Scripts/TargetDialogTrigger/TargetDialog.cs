using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetDialog : MonoBehaviour
{
    public enum TargetType
    {
        OpenObject,
        ModelBoardGetItem,
        PlayerPickPackage,
        ModelOpen,
        ClickOnTermometr,
        GetCells
    }

    private Aquarium Aquarium;
    private ModelBoard board;
    private Temperature termometr;
    private int numStateTermometr = 0;
    private OpenObject OpenObj;
    private int CountItems = 0;
    private PlayerInfo Player_Info;
    private AllPointerManager AllPointerManager;

    private bool OneTap = true;

    public List<DialogTarget> targets = new();

    private DialogManager DialogManager;

    [System.Serializable]
    public class DialogTarget
    {
        public TargetType TypeTarget = 0;
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
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].TypeTarget == TargetType.OpenObject)
            {
                OpenObj = GetComponent<OpenObject>();
            }
            else if (targets[i].TypeTarget == TargetType.ModelBoardGetItem)
            {
                board = GetComponent<ModelBoard>();
                CountItems = board.items.Count;
            }
            else if (targets[i].TypeTarget == TargetType.PlayerPickPackage)
            {
                Player_Info = GetComponent<PlayerInfo>();
            }
            else if (targets[i].TypeTarget == TargetType.ModelOpen)
            {
                board = GetComponent<ModelBoard>();
            }
            else if (targets[i].TypeTarget == TargetType.ClickOnTermometr)
            {
                termometr = GetComponent<Temperature>();
                numStateTermometr = termometr.numState;
            }
            else if (targets[i].TypeTarget == TargetType.GetCells)
            {
                Aquarium = GetComponent<Aquarium>();
            }
        }
        DialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        AllPointerManager = GameObject.Find("AllPointerManager").GetComponent<AllPointerManager>();
    }
    private void Update()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].TypeTarget == TargetType.OpenObject)
            {
                OpenObjectFunc(i);
            }
            else if (targets[i].TypeTarget == TargetType.ModelBoardGetItem)
            {
                ModelBoardGetItemFunc(i);
            }
            else if (targets[i].TypeTarget == TargetType.PlayerPickPackage)
            {
                PlayerPickPackageFunc(i);
            }
            else if (targets[i].TypeTarget == TargetType.ModelOpen)
            {
                ModelOpenFunc(i);
            }
            else if (targets[i].TypeTarget == TargetType.ClickOnTermometr)
            {
                TermometrClick(i);
            }
            else if (targets[i].TypeTarget == TargetType.GetCells)
            {
                CellsGet(i);
            }
        }
    }

    private void ActivateTarget(GameObject target, int Id)
    {
        var trgD = target.GetComponent<TargetDialog>();
        if (trgD)
        {
            for (int i = 0; i < trgD.targets.Count; i++)
            {
                if (trgD.targets[i].UniqId == Id)
                {
                    trgD.targets[i].Active = true;
                    break;
                }
            }
        }
    }

    private void OpenObjectFunc(int i)
    {
        if (OpenObj.ObjectIsOpen && OneTap)
        {
            OneTap = false;
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
                for (int j = 0; j < targets[i].NeedActivate.Count; j++)
                {
                    for (int k = 0; k < targets[i].NeedActivate[j].Ids.Count; k++)
                    {
                        ActivateTarget(targets[i].NeedActivate[j].obj, targets[i].NeedActivate[j].Ids[k]);
                    }
                }
            }
        }
        else if (!OpenObj.ObjectIsOpen && OneTap == false)
        {
            OneTap = true;
        }
    }
    private void ModelBoardGetItemFunc(int i)
    {
        if (board.items.Count > CountItems)
        {
            AllPointerManager.SetPointer(2);
            CountItems = board.items.Count;
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
                for (int j = 0; j < targets[i].NeedActivate.Count; j++)
                {
                    for (int k = 0; k < targets[i].NeedActivate[j].Ids.Count; k++)
                    {
                        ActivateTarget(targets[i].NeedActivate[j].obj, targets[i].NeedActivate[j].Ids[k]);
                    }
                }
            }
        }
    }
    private void PlayerPickPackageFunc(int i)
    {
        if (Player_Info.PlayerPickSometing && Player_Info.currentPickObject.GetComponent<PackageInfo>() && OneTap)
        {
            AllPointerManager.SetPointer(1);
            OneTap = false;
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
                for (int j = 0; j < targets[i].NeedActivate.Count; j++)
                {
                    for (int k = 0; k < targets[i].NeedActivate[j].Ids.Count; k++)
                    {
                        ActivateTarget(targets[i].NeedActivate[j].obj, targets[i].NeedActivate[j].Ids[k]);
                    }
                }
            }
        }
        else if (!Player_Info.PlayerPickSometing && OneTap == false)
        {
            OneTap = true;
        }
    }
    private void ModelOpenFunc(int i)
    {
        if (board.ModelOpen && OneTap)
        {
            AllPointerManager.SetPointer(3);
            OneTap = false;
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
                for (int j = 0; j < targets[i].NeedActivate.Count; j++)
                {
                    for (int k = 0; k < targets[i].NeedActivate[j].Ids.Count; k++)
                    {
                        ActivateTarget(targets[i].NeedActivate[j].obj, targets[i].NeedActivate[j].Ids[k]);
                    }
                }
            }
        }
        else if (!board.ModelOpen && !OneTap)
        {
            OneTap = true;
        }
    }
    private void TermometrClick(int i)
    {
        if (termometr.numState <= numStateTermometr)
        {
            numStateTermometr = termometr.numState;
        }
        else
        {
            numStateTermometr = termometr.numState;
            if (targets[i].Active)
            {
                if (targets[i].NewDialog)
                {
                    DialogManager.StartDialog(targets[i].NumDialog);
                    if (!targets[i].StayActiveAfter)
                    {
                        targets[i].Active = false;
                    }
                }
                else
                {
                    DialogManager.RunConditionSkip(targets[i].DialogTag);
                }
                for (int j = 0; j < targets[i].NeedActivate.Count; j++)
                {
                    for (int k = 0; k < targets[i].NeedActivate[j].Ids.Count; k++)
                    {
                        ActivateTarget(targets[i].NeedActivate[j].obj, targets[i].NeedActivate[j].Ids[k]);
                    }
                }
            }
        }
    }
    private void CellsGet(int i)
    {
        if (Aquarium.CountCells == 0)
        {
            if (targets[i].Active)
            {
                if (targets[i].NewDialog)
                {
                    DialogManager.StartDialog(targets[i].NumDialog);
                    if (!targets[i].StayActiveAfter)
                    {
                        targets[i].Active = false;
                    }
                }
                else
                {
                    DialogManager.RunConditionSkip(targets[i].DialogTag);
                }
                for (int j = 0; j < targets[i].NeedActivate.Count; j++)
                {
                    for (int k = 0; k < targets[i].NeedActivate[j].Ids.Count; k++)
                    {
                        ActivateTarget(targets[i].NeedActivate[j].obj, targets[i].NeedActivate[j].Ids[k]);
                    }
                }
            }
        }
    }
}
