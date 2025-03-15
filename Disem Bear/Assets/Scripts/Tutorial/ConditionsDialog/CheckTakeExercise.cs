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
    public void Init(DialogManager dialogManager, ExerciseManager exerciseManager)
    {
        this.dialogManager = dialogManager;
        this.exerciseManager = exerciseManager;
        this.exerciseManager.PlayerGetExercise.AddListener(CheckOrder);
    }

    public void CheckOrder(Exercise exercise)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].headerOrder == exercise.header)
            {
                dialogManager.RunConditionSkip(conditions[i].condition);
                break;
            }
        }
        for (int i = 0; i < startDialogs.Count; i++)
        {
            if (startDialogs[i].headerOrder == exercise.header && !dialogManager.IsDialogRun())
            {
                dialogManager.StartDialog(startDialogs[i].indexDialog);
                startDialogs.Remove(startDialogs[i]);
                break;
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
