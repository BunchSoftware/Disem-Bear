using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise
{
    private ExerciseReward exerciseReward;
    private bool isCompletedExercise = false;

    public bool GetIsCompletedExercise()
    {
        return isCompletedExercise;
    }

    public ExerciseReward DoneExercise()
    {
        isCompletedExercise = true;
        return exerciseReward;
    }
}
