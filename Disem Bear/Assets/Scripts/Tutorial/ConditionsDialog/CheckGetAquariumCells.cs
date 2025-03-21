using Game.Environment.Aquarium;
using Game.LDialog;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckGetAquariumCells : MonoBehaviour
{
    [SerializeField] private List<GetAquariumCellsCondition> conditions;
    [SerializeField] private List<GetAquariumCellsStartDialog> startDialogs;

    private DialogManager dialogManager;
    private Aquarium aquarium;

    public void Init(DialogManager dialogManager, Aquarium aquarium)
    {
        this.dialogManager = dialogManager;
        this.aquarium = aquarium;

        this.aquarium.GetAquariumCells.AddListener(CheckGetCells);
    }

    public void CheckGetCells(string nameCell, int countCells)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].nameCell == nameCell && countCells > 0)
            {
                dialogManager.SkipReplica();
                break;
            }
        }
        for (int i = 0; i < startDialogs.Count; i++)
        {
            if (startDialogs[i].nameCell == nameCell && countCells > 0 && !dialogManager.IsDialogRun())
            {
                dialogManager.StartDialog(startDialogs[i].indexDialog);
                startDialogs.Remove(startDialogs[i]);
                break;
            }
        }
    }

    [Serializable]
    public class GetAquariumCellsCondition
    {
        public string nameCell;
        [Space]
        public string condition = "None";
    }

    [Serializable]
    public class GetAquariumCellsStartDialog
    {
        public string nameCell;
        [Space]
        public int indexDialog = 0;
    }
}
