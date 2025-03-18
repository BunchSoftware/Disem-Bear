using External.DI;
using External.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UI.PlaneTablet.Shop;
using Unity.VisualScripting;
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
        private List<ExerciseGUI> exerciseGUIs = new List<ExerciseGUI>();

        public Action<Exercise> GetCurrentExercise;
        public Action<List<Reward>> GetExerciseRewards;

        private ExerciseGUI currentExerciseGUI;

        public void Init()
        {
            List<Exercise> exercises = new List<Exercise>();

            if (SaveManager.filePlayer.JSONPlayer.resources.exercises != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.exercises.Count; i++)
                {
                    Exercise exercise = FindExerciseToFileExercise(SaveManager.filePlayer.JSONPlayer.resources.exercises[i].indexExercise);
                    if (exercise != null)
                    {
                        exercise.isVisible = SaveManager.filePlayer.JSONPlayer.resources.exercises[i].isVisible;

                        prefab.name = $"Product {i}";
                        ExerciseGUI exerciseGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ExerciseGUI>();
                        exerciseGUIs.Add(exerciseGUI);
                        exercises.Add(exercise);
                    }
                }
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
                                if (exerciseGUIs[j] != exercise)
                                    exerciseGUIs[j].ExpandExercise(false);
                            }
                        }
                        else
                        {
                            PlayerGetExercise?.Invoke(exercise.GetExercise());
                            Sort(exercise);
                        } 
                    }, exercises[i], i);
                    exercise.ExpandExercise(false);

                    exerciseGUIs.Add(exercise);
                };
            }
        }


        private Exercise FindExerciseToFileExercise(int indexExercise)
        {
            return fileExercise.exercises[indexExercise];
        }

        public void SetVisibleExercies(int indexExercise, bool isVisible)
        {
            Exercise exercise = exerciseGUIs[indexExercise].GetExercise();
            if (exercise != null)
            {
                exercise.isVisible = isVisible;
                exerciseGUIs[indexExercise].UpdateData(exercise);

                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.exercises.Count; i++)
                {
                    if (SaveManager.filePlayer.JSONPlayer.resources.exercises[i].indexExercise == indexExercise)
                        SaveManager.filePlayer.JSONPlayer.resources.exercises[i].isVisible = isVisible;
                }

                SaveManager.UpdatePlayerFile();

                return;
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
            for (int j = 0; j < exerciseGUIs.Count; j++)
            {
                if (exerciseGUIs[j] != exercise && exerciseGUIs[j].GetExerciseCompletion() != TypeOfExerciseCompletion.Done)
                    exerciseGUIs[j].SetExerciseCompletion(TypeOfExerciseCompletion.NotDone);
                else if (exerciseGUIs[j].GetExerciseCompletion() == TypeOfExerciseCompletion.Run)
                    content.transform.GetChild(j).SetAsFirstSibling();
                else if (exerciseGUIs[j].GetExerciseCompletion() == TypeOfExerciseCompletion.Done)
                    content.transform.GetChild(j).SetAsLastSibling();
            }

            for (var i = 1; i < exerciseGUIs.Count; i++)
            {
                for (var j = 0; j < exerciseGUIs.Count - i; j++)
                {
                    if (exerciseGUIs[j].GetExerciseCompletion() < exerciseGUIs[j + 1].GetExerciseCompletion())
                    {
                        var temp = exerciseGUIs[j];
                        exerciseGUIs[j] = exerciseGUIs[j + 1];
                        exerciseGUIs[j + 1] = temp;
                    }
                }
            }
        }
    }
}
