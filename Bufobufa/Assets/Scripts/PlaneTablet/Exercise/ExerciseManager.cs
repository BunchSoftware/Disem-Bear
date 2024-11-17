using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExerciseManager : MonoBehaviour
{
    private List<Exercise> exercises = new List<Exercise>();

    private void Start()
    {
        exercises.Clear();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Exercise exercise;
            if (gameObject.transform.GetChild(i).TryGetComponent<Exercise>(out exercise))
            {
                exercise.Init((exercise) =>
                {
                    for(int j = 0; j < exercises.Count; j++)
                    {
                        if (exercises[j] != exercise)
                            exercises[j].ExpandExercise(false);
                    }
                });
                exercise.ExpandExercise(false);

                exercises.Add(exercise);
            };
        }
    }
}
