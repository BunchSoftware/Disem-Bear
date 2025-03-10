using System;
using System.Collections;
using System.Collections.Generic;
using Game.LDialog;
using UI.PlaneTablet.Exercise;
using UnityEngine;

public class TakeExerciseConditions : MonoBehaviour
{
    [SerializeField] private List<TakeExerciseCondition> conditions = new();

    private DialogManager dialogManager;
    private ExerciseManager exerciseManager;
    public void Init(DialogManager dialogManager, ExerciseManager exerciseManager)
    {
        this.dialogManager = dialogManager;
        this.exerciseManager = exerciseManager;
        exerciseManager.PlayerGetExercise.AddListener(CheckOrder);
    }

    public void CheckOrder(Exercise exercise)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].headerOrder == exercise.header)
            {
                dialogManager.RunConditionSkip(conditions[i].condition);
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
}
