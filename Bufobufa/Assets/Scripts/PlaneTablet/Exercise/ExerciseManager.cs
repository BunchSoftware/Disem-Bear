using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class JSONExercise
{
    public string header;
    public string reward;
    public string description;
    public string pathToAvatar;
}

public class ExerciseManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private List<Exercise> exercises = new List<Exercise>();
    private List<JSONExercise> JSONExercises = new List<JSONExercise>();

    private void Start()
    {
        exercises.Clear();

        string path = Application.streamingAssetsPath + "/" + "exercises.json";
        JSONExercises = JsonConvert.DeserializeObject<List<JSONExercise>>(File.ReadAllText(path));

        for (int i = 0; i < JSONExercises.Count; i++)
        {
            prefab.name = $"Exercise {i}";
            Instantiate(prefab, transform);
        }

        for (int i = 0; i < JSONExercises.Count; i++)
        {
            Exercise exercise;
            if (gameObject.transform.GetChild(i).TryGetComponent<Exercise>(out exercise))
            {
                exercise.Init((exercise, isExpandExercise) =>
                {
                    if(isExpandExercise)
                    {
                        for (int j = 0; j < exercises.Count; j++)
                        {
                            if (exercises[j] != exercise)
                                exercises[j].ExpandExercise(false);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < exercises.Count; j++)
                        {
                            if (exercises[j] != exercise && exercises[j].GetExerciseCompletion() != TypeOfExerciseCompletion.Done)
                                exercises[j].SetExerciseCompletion(TypeOfExerciseCompletion.NotDone);
                            else if (exercises[j].GetExerciseCompletion() == TypeOfExerciseCompletion.Run)
                                transform.GetChild(j).SetSiblingIndex(0);
                        }

                        for (var i = 1; i < exercises.Count; i++)
                        {
                            for (var j = 0; j < exercises.Count - i; j++)
                            {
                                if (exercises[j].GetExerciseCompletion() < exercises[j + 1].GetExerciseCompletion())
                                {
                                    var temp = exercises[j];
                                    exercises[j] = exercises[j + 1];
                                    exercises[j + 1] = temp;
                                }
                            }
                        }
                    }
                }, JSONExercises[i]);
                exercise.ExpandExercise(false);

                exercises.Add(exercise);
            };
        }
    }
}
