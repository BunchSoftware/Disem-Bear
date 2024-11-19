using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
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
    public UnityEvent<ExerciseReward> GetExerciseReward;
    private List<ExerciseGUI> exercises = new List<ExerciseGUI>();
    private List<JSONExercise> JSONExercises = new List<JSONExercise>();

    private ExerciseGUI currentExerciseGUI;

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
            ExerciseGUI exercise;
            if (gameObject.transform.GetChild(i).TryGetComponent<ExerciseGUI>(out exercise))
            {
                exercise.Init((exercise, isExpandExercise) =>
                {
                    currentExerciseGUI = exercise;

                    if (isExpandExercise)
                    {
                        for (int j = 0; j < exercises.Count; j++)
                        {
                            if (exercises[j] != exercise)
                                exercises[j].ExpandExercise(false);
                        }
                    }
                    else
                        Sort(exercise);
                }, JSONExercises[i], new Exercise());
                exercise.ExpandExercise(false);

                exercises.Add(exercise);
            };
        }
    }

    public void DoneCurrentExercise()
    {
        currentExerciseGUI.DoneExercise();
        Sort(currentExerciseGUI);
    }

    private void Sort(ExerciseGUI exercise)
    {
        for (int j = 0; j < exercises.Count; j++)
        {
            if (exercises[j] != exercise && exercises[j].GetExerciseCompletion() != TypeOfExerciseCompletion.Done)
                exercises[j].SetExerciseCompletion(TypeOfExerciseCompletion.NotDone);
            else if (exercises[j].GetExerciseCompletion() == TypeOfExerciseCompletion.Run)
                transform.GetChild(j).SetAsFirstSibling();
            else if (exercises[j].GetExerciseCompletion() == TypeOfExerciseCompletion.Done)
                transform.GetChild(j).SetAsLastSibling();
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
}
