using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise
{
    private bool isCompletedExercise = false;

    public ExerciseReward exerciseReward;
    public string rewardText;
    public string header;
    public string description;
    public string pathToAvatar;

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
