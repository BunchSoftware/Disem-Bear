using Game.LDialog;
using System;
using System.Collections.Generic;
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
            if (startDialogs[i].headerOrder == exercise.header && !dialogManager.IsDialogOn())
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
            if (dialogManager.GetCurrentIndexDialogPoint() == conditions[i].indexDialogPoint && dialogManager.GetCurrentIndexDialog() == conditions[i].indexDialog && conditions[i].headerOrder == exercise.header)
            {
                orderTaken = true;
                orderStartDialog = false;
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
                dialogManager.SkipReplica();
            }
        }
    }

    [Serializable]
    public class TakeExerciseCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public string headerOrder = "None";
    }

    [Serializable]
    public class TakeExerciseStartDialog
    {
        public string headerOrder = "None";
        [Space]
        public int indexDialog = 0;
    }
}
