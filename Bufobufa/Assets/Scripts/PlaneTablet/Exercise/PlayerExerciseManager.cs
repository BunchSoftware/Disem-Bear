using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExerciseManager : MonoBehaviour
{
    private ExerciseManager exerciseManager;

    private void Start()
    {
        exerciseManager = FindFirstObjectByType<ExerciseManager>();

        exerciseManager.GetCurrentExercise += (exercise) =>
        {

        };

        exerciseManager.GetExerciseReward += (reward) =>
        {
            print($"Награда {reward.money} монет");
        };
    }
}
