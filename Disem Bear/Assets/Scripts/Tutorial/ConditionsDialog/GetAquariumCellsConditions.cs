using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.Aquarium;
using Game.LDialog;
using UnityEngine;

public class GetAquariumCellsConditions : MonoBehaviour
{
    [SerializeField] private List<GetAquariumCellsCondition> conditions;

    private DialogManager dialogManager;
    private Aquarium aquarium;

    public void Init(DialogManager dialogManager, Aquarium aquarium)
    {
        this.dialogManager = dialogManager;
        this.aquarium = aquarium;

        aquarium.GetAquariumCells.AddListener(CheckGetCells);
    }

    public void CheckGetCells(string nameCell, int countCells)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].nameCell == nameCell && countCells > 0)
            {
                dialogManager.RunConditionSkip(conditions[i].condition);
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
}
