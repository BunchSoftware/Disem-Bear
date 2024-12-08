using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Exercise
{
    private bool isCompletedExercise = false;

    public ExerciseReward exerciseReward;
    public string rewardText;
    public string header;
    [TextArea(10, 100)]
    public string description;
    public Sprite avatar;

    public string conditionExercise;

    public bool GetIsCompletedExercise()
    {
        return isCompletedExercise;
    }

    public ExerciseReward DoneExercise(string messageCondition)
    {
        if(conditionExercise == messageCondition)
        {
            isCompletedExercise = true;
            return exerciseReward;
        }
        else
            return null;
    }
}
