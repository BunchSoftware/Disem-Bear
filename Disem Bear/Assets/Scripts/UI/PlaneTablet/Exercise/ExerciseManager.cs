using External.DI;
using External.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UI.PlaneTablet.Shop;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.PlaneTablet.Exercise
{
    [Serializable]
    public class ExerciseManager : IUpdateListener
    {
        public UnityEvent<Exercise> PlayerGetExercise;
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject content;
        [SerializeField] private FileExercise fileExercise;
        [SerializeField] private List<TypeMachineDispensingProduct> typeGiveProducts;
        private List<ExerciseGUI> exercisesGUI = new List<ExerciseGUI>();

        public Action<Exercise> GetCurrentExercise;
        public Action<List<Reward>> GetExerciseRewards;

        private ExerciseGUI currentExerciseGUI;

        public void Init()
        {
            List<Exercise> exercises = fileExercise.exercises;


            if (SaveManager.filePlayer.JSONPlayer.resources.exercises != null)
            {
                if (SaveManager.filePlayer.JSONPlayer.resources.exercises.Count == 0)
                {
                    SaveManager.filePlayer.JSONPlayer.resources.exercises = new List<ExerciseData>();

                    for (int j = 0; j < exercises.Count; j++)
                    {
                        SaveManager.filePlayer.JSONPlayer.resources.exercises.Add(new ExerciseData()
                        {
                            indexExercise = j,
                            isGetPackage = false,
                            typeOfExerciseCompletion = TypeOfExerciseCompletion.NotDone,
                        });
                    }
                }
            }

            for (int i = 0; i < exercises.Count; i++)
            {
                prefab.name = $"Exercise {i}";
                GameObject.Instantiate(prefab, content.transform);
            }

            for (int i = 0; i < exercises.Count; i++)
            {
                ExerciseGUI exercise;

                if (content.gameObject.transform.GetChild(i).TryGetComponent<ExerciseGUI>(out exercise))
                {
                    exercise.Init(this, (exercise, isExpandExercise) =>
                    {
                        PlayerGetExercise?.Invoke(exercise.GetExercise());

                        currentExerciseGUI = exercise;
                        GetCurrentExercise?.Invoke(currentExerciseGUI.GetExercise());

                        if (isExpandExercise)
                        {
                            for (int j = 0; j < exercises.Count; j++)
                            {
                                if (exercisesGUI[j] != exercise)
                                    exercisesGUI[j].ExpandExercise(false);
                            }
                        }
                        else
                            Sort(exercise);
                    }, exercises[i], i);
                    exercise.ExpandExercise(false);

                    exercisesGUI.Add(exercise);
                };
            }
        }

        public void OnUpdate(float deltaTime)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.gameObject.GetComponent<RectTransform>());
        }

        public void DoneCurrentExercise(string messageExercise)
        {
            List<Reward> exerciseRewards = currentExerciseGUI.DoneExercise(messageExercise);
            GetExerciseRewards?.Invoke(exerciseRewards);
            GiveRewards(exerciseRewards);
            Sort(currentExerciseGUI);
        }

        private void GiveRewards(List<Reward> exerciseRewards)
        {
            if (typeGiveProducts != null)
            {
                for (int i = 0; i < typeGiveProducts.Count; i++)
                {
                    for (int j = 0; j < exerciseRewards.Count; j++)
                    {
                        if (typeGiveProducts[i].typeMachineDispensingProduct == exerciseRewards[j].typeMachineDispensingReward)
                        {
                            typeGiveProducts[i].OnGetReward.Invoke(exerciseRewards[j]);
                        }
                    }
                }
            }
        }

        public void GivePackage(Exercise exercise)
        {
            if (typeGiveProducts != null)
            {
                for (int i = 0; i < typeGiveProducts.Count; i++)
                {
                    for (int j = 0; j < exercise.exerciseRewards.Count; j++)
                    {
                        if (typeGiveProducts[i].typeMachineDispensingProduct == exercise.exerciseRewards[j].typeMachineDispensingReward)
                        {
                            typeGiveProducts[i].OnGetReward?.Invoke(exercise.exerciseRewards[j]);
                        }
                    }
                }
            }
        }

        private void Sort(ExerciseGUI exercise)
        {
            for (int j = 0; j < exercisesGUI.Count; j++)
            {
                if (exercisesGUI[j] != exercise && exercisesGUI[j].GetExerciseCompletion() != TypeOfExerciseCompletion.Done)
                    exercisesGUI[j].SetExerciseCompletion(TypeOfExerciseCompletion.NotDone);
                else if (exercisesGUI[j].GetExerciseCompletion() == TypeOfExerciseCompletion.Run)
                    content.transform.GetChild(j).SetAsFirstSibling();
                else if (exercisesGUI[j].GetExerciseCompletion() == TypeOfExerciseCompletion.Done)
                    content.transform.GetChild(j).SetAsLastSibling();
            }

            for (var i = 1; i < exercisesGUI.Count; i++)
            {
                for (var j = 0; j < exercisesGUI.Count - i; j++)
                {
                    if (exercisesGUI[j].GetExerciseCompletion() < exercisesGUI[j + 1].GetExerciseCompletion())
                    {
                        var temp = exercisesGUI[j];
                        exercisesGUI[j] = exercisesGUI[j + 1];
                        exercisesGUI[j + 1] = temp;
                    }
                }
            }
        }
    }
}
