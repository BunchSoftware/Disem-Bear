using External.DI;
using External.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UI.PlaneTablet.Shop;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PlaneTablet.Exercise
{
    [Serializable]
    public class ExerciseManager : IUpdateListener
    {
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject content;
        [SerializeField] private FileExercise fileExercise;
        [SerializeField] private List<TypeMachineDispensingProduct> typeGiveProducts;
        private List<ExerciseGUI> exercisesGUI = new List<ExerciseGUI>();

        public Action<Exercise> GetCurrentExercise;
        public Action<ExerciseReward> GetExerciseReward;

        private ExerciseGUI currentExerciseGUI;

        public void Init(SaveManager saveManager)
        {
            this.saveManager = saveManager;
            List<Exercise> exercises = fileExercise.exercises;


            if (saveManager.filePlayer.JSONPlayer.resources.exerciseSaves != null)
            {
                if (saveManager.filePlayer.JSONPlayer.resources.exerciseSaves.Count == 0)
                {
                    saveManager.filePlayer.JSONPlayer.resources.exerciseSaves = new List<ExerciseSave>();

                    for (int j = 0; j < exercises.Count; j++)
                    {
                        saveManager.filePlayer.JSONPlayer.resources.exerciseSaves.Add(new ExerciseSave()
                        {
                            indexExercise = j,
                            isGetPackage = false,
                            typeOfExerciseCompletion = TypeOfExerciseCompletion.NotDone,
                        });
                    }

                    saveManager.UpdatePlayerFile();
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
                    }, exercises[i], i, saveManager);
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
            ExerciseReward exerciseReward = currentExerciseGUI.DoneExercise(messageExercise);
            GetExerciseReward?.Invoke(exerciseReward);
            GiveReward(exerciseReward);
            Sort(currentExerciseGUI);
        }

        private void GiveReward(ExerciseReward exerciseReward)
        {
            if (typeGiveProducts != null)
            {
                for (int i = 0; i < typeGiveProducts.Count; i++)
                {
                    if (typeGiveProducts[i].typeMachineDispensingProduct == exerciseReward.typeMachineDispensingReward)
                    {
                        typeGiveProducts[i].OnGetProduct.Invoke(exerciseReward.typeReward);
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
                    if (typeGiveProducts[i].typeMachineDispensingProduct == exercise.typeMachineDispensingPackage)
                    {
                        typeGiveProducts[i].OnGetProduct.Invoke(exercise.typePackage);
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
