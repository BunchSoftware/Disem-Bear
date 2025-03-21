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
        [SerializeField] private ExerciseDatabase exerciseDatabase;
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

        private Queue<DispensingTask> dispensingTasks = new Queue<DispensingTask>();

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
                    Exercise exercise = FindExerciseToFileExercise(SaveManager.filePlayer.JSONPlayer.resources.exercises[i].nameExercise);
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
                ExerciseGUI exerciseGUI;

                if (content.gameObject.transform.GetChild(i).TryGetComponent<ExerciseGUI>(out exerciseGUI))
                {
                    exerciseGUI.Init(this, (exercise, isExpandExercise) =>
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
                    exerciseGUI.ExpandExercise(false);
                };
            }

            Exercise exerciseRandom = RandomExerciseWithoutRepeat(null);
            if (exerciseRandom != null)
            {
                AddExercise(exerciseRandom);
            }

            tv.OnTVClose.AddListener(() =>
            {
                if (randomMail == null)
                    randomMail = context.StartCoroutine(IRandomMail());

                if (dispensingTasks.Count > 0)
                {
                    DispensingTask dispensingTask = dispensingTasks.Dequeue();

                    if (dispensingTask.exerciseItem != null)
                        dispensingTask.typeMachineDispensingProduct.OnGetExerciseItem?.Invoke(dispensingTask.exerciseItem);
                }
            });
        }


        private Exercise FindExerciseToFileExercise(string nameExercise)
        {
            Exercise exercise = null;

            for (int i = 0; i < exerciseDatabase.exercises.Count; i++)
            {
                if (exerciseDatabase.exercises[i].exercise.nameExercise == nameExercise)
                {
                    exercise = exerciseDatabase.exercises[i].exercise;
                    break;
                }
            }
            return exercise;
        }

        public void SetVisibleExercies(string nameExercise, bool isVisible)
        {
            Exercise exercise = null;
            ExerciseGUI exerciseGUI = null;

            for (int i = 0; i < exerciseGUIs.Count; i++)
            {
                if (exerciseGUIs[i].GetExercise().nameExercise == nameExercise)
                {
                    exercise = exerciseGUIs[i].GetExercise();
                    exerciseGUI = exerciseGUIs[i];
                }
            }

            if (exercise != null)
            {
                exercise.isVisible = isVisible;
                exerciseGUI.UpdateData(exercise);

                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.exercises.Count; i++)
                {
                    if (SaveManager.filePlayer.JSONPlayer.resources.exercises[i].nameExercise == nameExercise)
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

        public void AddExercise(Exercise exercise)
        {
            ExerciseData exerciseData = new ExerciseData();

            exerciseData.isGetExerciesItems = false;
            exerciseData.isVisible = true;
            exerciseData.nameExercise = exercise.nameExercise;
            exerciseData.typeOfExerciseCompletion = TypeOfExerciseCompletion.NotDone;

            SaveManager.filePlayer.JSONPlayer.resources.exercises.Add(exerciseData);
            exercise.isVisible = SaveManager.filePlayer.JSONPlayer.resources.exercises[SaveManager.filePlayer.JSONPlayer.resources.exercises.Count-1].isVisible;

            if (exercise.isMail)
                SaveManager.filePlayer.JSONPlayer.resources.exercises[SaveManager.filePlayer.JSONPlayer.resources.exercises.Count - 1].typeOfExerciseCompletion = TypeOfExerciseCompletion.Run;

            ExerciseGUI exerciseGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ExerciseGUI>();
            exerciseGUI.name = $"Exercise {exerciseGUIs.Count}";
            exerciseGUIs.Add(exerciseGUI);

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
            }, exercise, exerciseGUIs.Count-1);
            exerciseGUI.ExpandExercise(false);

            SaveManager.UpdatePlayerFile();
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
                            typeGiveProducts[i].OnGetReward?.Invoke(exerciseRewards[j]);    

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
                            DispensingTask dispensingTask = new DispensingTask();
                            dispensingTask.typeMachineDispensingProduct = typeGiveProducts[i];
                            dispensingTask.exerciseItem = exercise.exerciseItems[j];
                            dispensingTasks.Enqueue(dispensingTask);

                            toastManager.ShowToast("Пожалуйста, заберите квестовый предмет");
                            Debug.Log("Игрок получил квестовый предмет");                           
                        }
                    }
                }
            }
        }

        public Exercise RandomExerciseWithoutRepeat(Exercise exercise)
        {
            List<Exercise> randomExercises = new List<Exercise>();
            Exercise exerciseResult = null;
            
            if(exercise != null)
            {
                for (int i = 0; i < exerciseDatabase.exercises.Count; i++)
                {
                    if (!exerciseDatabase.exercises[i].exercise.isMail && exerciseDatabase.exercises[i].exercise.isRandom
                        && exerciseDatabase.exercises[i].exercise.nameExercise != exercise.nameExercise)
                        randomExercises.Add(exerciseDatabase.exercises[i].exercise);
                }
            }
            else
            {
                for (int i = 0; i < exerciseDatabase.exercises.Count; i++)
                {
                    if (!exerciseDatabase.exercises[i].exercise.isMail && exerciseDatabase.exercises[i].exercise.isRandom)
                        randomExercises.Add(exerciseDatabase.exercises[i].exercise);
                }
            }

            int indexRandom = 0;
            bool isExit = false;
            if (randomExercises.Count > 0)
            {
                while (!isExit)
                {
                    indexRandom = UnityEngine.Random.Range(0, randomExercises.Count);
                    for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.exercises.Count; i++)
                    {
                        if (SaveManager.filePlayer.JSONPlayer.resources.exercises[i].nameExercise == randomExercises[indexRandom].nameExercise)
                            break;

                        isExit = true;
                        break;
                    }
                }
                exerciseResult = randomExercises[indexRandom];
                if(exercise != null)
                {
                    for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.exercises.Count; i++)
                    {
                        if (SaveManager.filePlayer.JSONPlayer.resources.exercises[i].nameExercise == exercise.nameExercise)
                        {
                            ExerciseData exerciseData = new ExerciseData();

                            exerciseData.isGetExerciesItems = false;
                            exerciseData.isVisible = true;
                            exerciseData.nameExercise = exerciseResult.nameExercise;
                            exerciseData.typeOfExerciseCompletion = TypeOfExerciseCompletion.NotDone;

                            SaveManager.filePlayer.JSONPlayer.resources.exercises[i] = exerciseData;
                            SaveManager.UpdatePlayerFile();
                        }
                    }
                }
            }
            return exerciseResult;
        }

        IEnumerator IRandomMail()
        {
            Debug.Log("Вам сейчас прилетит письмо !");
            yield return new WaitForSeconds(intervalTimeAppearanceMail);
            if (countMail < maxMail)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeAppearanceMail, maxTimeAppearanceMail));

                List<Exercise> mailExercises = new List<Exercise>();

                for (int i = 0; i < exerciseDatabase.exercises.Count; i++)
                {
                    if (exerciseDatabase.exercises[i].exercise.isMail && exerciseDatabase.exercises[i].exercise.isRandom)
                        mailExercises.Add(exerciseDatabase.exercises[i].exercise);
                }

                int indexRandom = 0;
                bool isExit = false;
                if (mailExercises.Count > 0)
                {
                    while (!isExit)
                    {
                        indexRandom = UnityEngine.Random.Range(0, mailExercises.Count);
                        for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.exercises.Count; i++)
                        {
                            if (SaveManager.filePlayer.JSONPlayer.resources.exercises[i].nameExercise == mailExercises[indexRandom].nameExercise)
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

                        for (int i = 0; i < exerciseDatabase.exercises.Count; i++)
                        {
                            if (exerciseDatabase.exercises[i].exercise == exercise)
                            {
                                exerciseData.nameExercise = exerciseDatabase.exercises[i].exercise.nameExercise;
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
