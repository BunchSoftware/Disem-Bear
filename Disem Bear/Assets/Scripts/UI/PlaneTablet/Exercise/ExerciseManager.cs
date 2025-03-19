using External.DI;
using External.Storage;
using Newtonsoft.Json;
using System;
using System.Collections;
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
        [SerializeField] private int maxMail = 3;
        [SerializeField] private float minTimeAppearanceMail = 0;
        [SerializeField] private float maxTimeAppearanceMail = 60;
        [SerializeField] private float intervalTimeAppearanceMail = 120;

        private int countMail = 0;

        private List<ExerciseGUI> exerciseGUIs = new List<ExerciseGUI>();

        public Action<Exercise> GetCurrentExercise;
        public Action<List<Reward>> GetExerciseRewards;

        private ExerciseGUI currentExerciseGUI;

        private MonoBehaviour context;
        private ToastManager toastManager;
        private TV tv;
        private Coroutine randomMail;

        public void Init(TV tv, ToastManager toastManager, MonoBehaviour context)
        {
            countMail = SaveManager.filePlayer.JSONPlayer.resources.countMail;
            this.toastManager = toastManager;
            this.context = context;
            this.tv = tv;
            List<Exercise> exercises = new List<Exercise>();

            if (SaveManager.filePlayer.JSONPlayer.resources.exercises != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.exercises.Count; i++)
                {
                    Exercise exercise = FindExerciseToFileExercise(SaveManager.filePlayer.JSONPlayer.resources.exercises[i].indexExercise);
                    if (exercise != null)
                    {
                        exercise.isVisible = SaveManager.filePlayer.JSONPlayer.resources.exercises[i].isVisible;

                        if (exercise.isMail)
                            SaveManager.filePlayer.JSONPlayer.resources.exercises[i].typeOfExerciseCompletion = TypeOfExerciseCompletion.Run;

                        ExerciseGUI exerciseGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ExerciseGUI>();
                        exerciseGUI.name = $"Exercise {i}";
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
                };
            }

            tv.OnTVClose.AddListener(() =>
            {
                if (randomMail == null)
                    randomMail = context.StartCoroutine(IRandomMail());
            });
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
                            Debug.Log("Игрок получил награду за квест");
                            toastManager.ShowToast("Пожалуйста, заберите награду за квест");
                        }
                    }
                }
            }
        }

        public void GiveExerciseItem(Exercise exercise)
        {
            if (typeGiveProducts != null)
            {
                for (int i = 0; i < typeGiveProducts.Count; i++)
                {
                    for (int j = 0; j < exercise.exerciseItems.Count; j++)
                    {
                        if (typeGiveProducts[i].typeMachineDispensingProduct == exercise.exerciseItems[j].typeMachineDispensingItem)
                        {
                            toastManager.ShowToast("Пожалуйста, заберите квестовый предмет");
                            Debug.Log("Игрок получил квестовый предмет");
                            typeGiveProducts[i].OnGetExerciseItem?.Invoke(exercise.exerciseItems[j]);
                        }
                    }
                }
            }
        }

        IEnumerator IRandomMail()
        {
            Debug.Log("Вам сейчас прилетит письмо !");
            yield return new WaitForSeconds(intervalTimeAppearanceMail);
            if (countMail < maxMail)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeAppearanceMail, maxTimeAppearanceMail));

                List<Exercise> mailExercises = new List<Exercise>();

                for (int i = 0; i < fileExercise.exercises.Count; i++)
                {
                    if (fileExercise.exercises[i].isMail && fileExercise.exercises[i].isRandomMail)
                        mailExercises.Add(fileExercise.exercises[i]);
                }

                int indexRandom = 0;
                bool isExit = false;
                while (!isExit)
                {
                    indexRandom = UnityEngine.Random.Range(0, mailExercises.Count);
                    for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.exercises.Count; i++)
                    {
                        if (SaveManager.filePlayer.JSONPlayer.resources.exercises[i].indexExercise == indexRandom)
                            break;

                        isExit = true;
                        break;
                    }
                }
                Exercise exercise = mailExercises[indexRandom];

                if (exercise.isMail)
                {
                    countMail++;
                    SaveManager.filePlayer.JSONPlayer.resources.countMail = countMail;
                }

                if (exercise != null && exercise.isMail)
                {
                    ExerciseGUI exerciseGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ExerciseGUI>();
                    exerciseGUI.name = $"Exercise {exerciseGUIs.Count}";

                    ExerciseData exerciseData = new ExerciseData();

                    for (int i = 0; i < fileExercise.exercises.Count; i++)
                    {
                        if (fileExercise.exercises[i] == exercise)
                        {
                            exerciseData.indexExercise = i;
                            Debug.Log(152);
                            break;
                        }
                    }
                    exerciseData.isVisible = true;
                    exerciseData.isGetExerciesItems = false;
                    exerciseData.typeOfExerciseCompletion = TypeOfExerciseCompletion.NotDone;

                    SaveManager.filePlayer.JSONPlayer.resources.exercises.Add(exerciseData);

                    exerciseGUI.Init(this, (exercise, isExpandExercise) =>
                    {
                        currentExerciseGUI = exercise;
                        GetCurrentExercise?.Invoke(currentExerciseGUI.GetExercise());

                        if (isExpandExercise)
                        {
                            for (int j = 0; j < exerciseGUIs.Count; j++)
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
                    }, exercise, exerciseGUIs.Count);

                    exerciseGUI.ExpandExercise(false);
                    exerciseGUIs.Add(exerciseGUI);

                    randomMail = null;

                    toastManager.ShowToast("Вам пришло новое письмо, пожалуйста проверьте почтовый ящик");
                    SaveManager.UpdatePlayerFile();
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
