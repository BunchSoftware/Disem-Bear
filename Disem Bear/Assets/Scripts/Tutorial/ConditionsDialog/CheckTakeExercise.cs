using System;
using System.Collections;
using System.Collections.Generic;
using Game.LDialog;
using UI.PlaneTablet.Exercise;
using UnityEngine;

public class CheckTakeExercise : MonoBehaviour
{
    [SerializeField] private List<TakeExerciseCondition> conditions = new();
    [SerializeField] private List<TakeExerciseStartDialog> startDialogs = new();

    private DialogManager dialogManager;
    private ExerciseManager exerciseManager;
    private TV TV;

    private bool orderTaken = false;
    private bool orderStartDialog = false;
    private string conditionOrder = "None";
    private int indexDialogPoint = 0;

    public void Init(DialogManager dialogManager, ExerciseManager exerciseManager, TV TV)
    {
        this.TV = TV;
        this.TV.OnTVClose.AddListener(CheckOrderOnExitTV);
        this.dialogManager = dialogManager;
        this.exerciseManager = exerciseManager;
        this.exerciseManager.PlayerGetExercise.AddListener(CheckOrder);
    }

    public void CheckOrder(Exercise exercise)
    {
        for (int i = 0; i < startDialogs.Count; i++)
        {
            if (startDialogs[i].headerOrder == exercise.header && !dialogManager.IsDialogRun())
            {
                orderTaken = true;
                orderStartDialog = true;
                indexDialogPoint = startDialogs[i].indexDialog;
                startDialogs.Remove(startDialogs[i]);
                return;
            }
        }
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].headerOrder == exercise.header)
            {
                orderTaken = true;
                orderStartDialog = false;
                conditionOrder = conditions[i].condition;
                break;
            }
        }
    }

    public void CheckOrderOnExitTV()
    {
        if (orderTaken)
        {
            orderTaken = false;
            if (orderStartDialog)
            {
                dialogManager.StartDialog(indexDialogPoint);
            }
            else
            {
                dialogManager.RunConditionSkip(conditionOrder);
            }
        }
    }

    [Serializable]
    public class TakeExerciseCondition
    {
        public string headerOrder = "None";
        [Space]
        public string condition = "None";
    }

    [Serializable]
    public class TakeExerciseStartDialog
    {
        public string headerOrder = "None";
        [Space]
        public int indexDialog = 0;
    }
}
