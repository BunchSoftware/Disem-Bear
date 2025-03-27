
using External.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.PlaneTablet.Exercise
{
    public enum TypeOfExerciseCompletion
    {
        NotDone = 2,
        Run = 3,
        Done = 1
    }

    public class ExerciseGUI : MonoBehaviour
    {
        [SerializeField] private GameObject prefabReward;
        [SerializeField] private GameObject prefabRequement;
        [SerializeField] private RectTransform description;
        [SerializeField] private Button exerciseButton;
        [SerializeField] private Image checkMark;

        [Header("Кнопки выполнения задания")]
        [SerializeField] private Button runButton;
        [SerializeField] private Button executionButton;
        [SerializeField] private Button doneButton;

        [Header("Основные элементы заданий")]
        [SerializeField] private Text headerText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Image avatar;
        [SerializeField] private Image arrowUp;
        [SerializeField] private Image arrowDown;
        [SerializeField] private Text headerExerciseRequirements;
        [SerializeField] private GameObject contentRewards;
        [SerializeField] private GameObject contentRequements;

        [Header("Цветовой бортик с цветами")]
        [SerializeField] private Image background;
        [SerializeField] private Color colorNotDoneExerciseBackground;
        [SerializeField] private Color colorDoneExerciseBackground;
        [SerializeField] private Color colorRunExerciseBackground;

        [Header("Кнопка обновления квеста")]
        [SerializeField] private Button resetButton;

        private TypeOfExerciseCompletion currentExerciseCompletion = TypeOfExerciseCompletion.NotDone;
        private bool isExpandExercise = false;
        private Exercise exercise;
        private ExerciseManager exerciseManager;
        private List<ExerciseRewardGUI> exerciseRewardGUIs = new List<ExerciseRewardGUI>();
        private List<ExerciseRequementGUI> exerciseRequirementsGUIs = new List<ExerciseRequementGUI>();
        private int indexExercise = 0;
        private const int MaxObjectFall = 3;

        private ToastManager toastManager;
        public void Init(ExerciseManager exerciseManager, ToastManager toastManager, Action<ExerciseGUI, bool> ActionExercise, Exercise exercise, int indexExercise)
        {
            this.toastManager = toastManager;
            this.exerciseManager = exerciseManager;
            this.exercise = exercise;
            this.indexExercise = indexExercise;

            SetExerciseCompletion(SaveManager.playerDatabase.JSONPlayer.resources.exercises[indexExercise].typeOfExerciseCompletion);

            exerciseButton.onClick.AddListener(() =>
            {
                ActionExercise.Invoke(this, true);

                if (isExpandExercise)
                    ExpandExercise(false);
                else
                    ExpandExercise(true);
            });

            runButton.onClick.AddListener(() =>
            {
                if (exerciseManager.CountDispensingTasks < MaxObjectFall)
                {
                    SetExerciseCompletion(TypeOfExerciseCompletion.Run);
                    ActionExercise.Invoke(this, false);
                    if (!SaveManager.playerDatabase.JSONPlayer.resources.exercises[indexExercise].isGetExerciesItems)
                        exerciseManager.GiveExerciseItem(exercise);

                    UpdateData(exercise);

                    SaveManager.playerDatabase.JSONPlayer.resources.exercises[indexExercise].isGetExerciesItems = true;
                    SaveManager.UpdatePlayerDatabase();
                }
                else
                {
                    toastManager.ShowToast("Достигнуто максимальное количество предметов на выдачу");
                }
            });

            resetButton.onClick.AddListener(() =>
            {
                exercise = exerciseManager.RandomExerciseWithoutRepeat(exercise);
                UpdateData(exercise);
            });

            UpdateData(exercise);
        }

        public void UpdateData(Exercise exercise)
        {
            this.exercise = exercise;
            gameObject.SetActive(exercise.isVisible);
            SetExerciseCompletion(SaveManager.playerDatabase.JSONPlayer.resources.exercises[indexExercise].typeOfExerciseCompletion);

            if (currentExerciseCompletion == TypeOfExerciseCompletion.NotDone && exercise.isRandom && !exercise.isMail)
                resetButton.gameObject.SetActive(true);
            else
                resetButton.gameObject.SetActive(false);

            if(exerciseRequirementsGUIs.Count == 0)
            {
                if (exercise.exerciseRequirements != null) {
                    for (int i = 0; i < exercise.exerciseRequirements.Count; i++)
                    {
                        ExerciseRequementGUI exerciseItemGUI = Instantiate(prefabRequement, contentRequements.transform).GetComponent<ExerciseRequementGUI>();
                        exerciseItemGUI.countRequementText.text = $"{exercise.exerciseRequirements[i].countRequirement}x";
                        exerciseItemGUI.avatarRequement.sprite = exercise.exerciseRequirements[i].avatarRequirement;
                        exerciseRequirementsGUIs.Add(exerciseItemGUI);
                    }
                }

                if (exercise.exerciseRequirements == null || exercise.exerciseRequirements.Count == 0)
                    headerExerciseRequirements.gameObject.SetActive(false);
                else
                    headerExerciseRequirements.gameObject.SetActive(true);
            }
            else
            {
                for (int i = 0; i < exerciseRequirementsGUIs.Count; i++)
                {
                    Destroy(exerciseRequirementsGUIs[i].gameObject);
                }
                exerciseRequirementsGUIs.Clear();

                if (exercise.exerciseRequirements.Count == 0)
                    headerExerciseRequirements.gameObject.SetActive(false);
                else
                    headerExerciseRequirements.gameObject.SetActive(true);

                for (int i = 0; i < exercise.exerciseRequirements.Count; i++)
                {
                    ExerciseRequementGUI exerciseItemGUI = Instantiate(prefabRequement, contentRequements.transform).GetComponent<ExerciseRequementGUI>();
                    exerciseItemGUI.countRequementText.text = $"{exercise.exerciseRequirements[i].countRequirement}x";
                    exerciseItemGUI.avatarRequement.sprite = exercise.exerciseRequirements[i].avatarRequirement;
                    exerciseRequirementsGUIs.Add(exerciseItemGUI);
                }
            }

            if (exerciseRewardGUIs.Count == 0)
            {
                if (exercise.exerciseRewards != null)
                {
                    for (int i = 0; i < exercise.exerciseRewards.Count; i++)
                    {
                        ExerciseRewardGUI exerciseRewardGUI = Instantiate(prefabReward, contentRewards.transform).GetComponent<ExerciseRewardGUI>();
                        exerciseRewardGUI.countRewardText.text = $"{exercise.exerciseRewards[i].countReward}x";
                        exerciseRewardGUI.avatarReward.sprite = exercise.exerciseRewards[i].avatarReward;
                        exerciseRewardGUIs.Add(exerciseRewardGUI);
                    }
                }
            }
            else
            {
                for (int i = 0; i < exerciseRewardGUIs.Count; i++)
                {
                    Destroy(exerciseRewardGUIs[i].gameObject);
                }
                exerciseRewardGUIs.Clear();

                for (int i = 0; i < exercise.exerciseRewards.Count; i++)
                {
                    ExerciseRewardGUI exerciseRewardGUI = Instantiate(prefabReward, contentRewards.transform).GetComponent<ExerciseRewardGUI>();
                    exerciseRewardGUI.countRewardText.text = $"{exercise.exerciseRewards[i].countReward}x";
                    exerciseRewardGUI.avatarReward.sprite = exercise.exerciseRewards[i].avatarReward;
                    exerciseRewardGUIs.Add(exerciseRewardGUI);
                }
            }


            headerText.text = exercise.header;
            descriptionText.text = exercise.description;
            avatar.sprite = exercise.avatar;
            avatar.preserveAspect = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        }

        public void ExpandExercise(bool isExpandExercise)
        {
            this.isExpandExercise = isExpandExercise;
            arrowDown.gameObject.SetActive(isExpandExercise);
            arrowUp.gameObject.SetActive(!isExpandExercise);
            if (description != null)
            {
                description.gameObject.SetActive(isExpandExercise);
            }
            else
                throw new System.Exception("Ошибка ! Добавьте обьект Description");
        }

        public void SetExerciseCompletion(TypeOfExerciseCompletion exerciseCompletion)
        {
            currentExerciseCompletion = exerciseCompletion;

            for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.exercises.Count; i++)
            {
                if (SaveManager.playerDatabase.JSONPlayer.resources.exercises[i].nameExercise == exercise.nameExercise)
                {
                    SaveManager.playerDatabase.JSONPlayer.resources.exercises[i].typeOfExerciseCompletion = exerciseCompletion;
                    break;
                }
            }
            switch (exerciseCompletion)
            {
                case TypeOfExerciseCompletion.NotDone:
                    {
                        background.color = colorNotDoneExerciseBackground;
                        if (!exercise.isMail)
                        {
                            runButton.gameObject.SetActive(true);
                            doneButton.gameObject.SetActive(false);
                            executionButton.gameObject.SetActive(false);
                        }
                        else
                        {
                            runButton.gameObject.SetActive(false);
                            doneButton.gameObject.SetActive(false);
                            executionButton.gameObject.SetActive(false);
                        }
                        checkMark.gameObject.SetActive(false);
                        break;
                    }
                case TypeOfExerciseCompletion.Run:
                    {
                        background.color = colorRunExerciseBackground;
                        if (!exercise.isMail)
                        {
                            runButton.gameObject.SetActive(false);
                            doneButton.gameObject.SetActive(false);
                            executionButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            runButton.gameObject.SetActive(false);
                            doneButton.gameObject.SetActive(false);
                            executionButton.gameObject.SetActive(false);
                        }
                        checkMark.gameObject.SetActive(false);
                        break;
                    }
                case TypeOfExerciseCompletion.Done:
                    {
                        background.color = colorDoneExerciseBackground;
                        if (!exercise.isMail)
                        {
                            runButton.gameObject.SetActive(false);
                            doneButton.gameObject.SetActive(true);
                            executionButton.gameObject.SetActive(false);
                        }
                        else
                        {
                            runButton.gameObject.SetActive(false);
                            doneButton.gameObject.SetActive(false);
                            executionButton.gameObject.SetActive(false);
                        }
                        checkMark.gameObject.SetActive(true);
                        exercise.isVisible = false;

                        for (int i = 0; i < exercise.newExercises.Count; i++)
                        {
                            exerciseManager.AddExercise(exercise.newExercises[i]);
                        }

                        break;
                    }
            }
            SaveManager.UpdatePlayerDatabase();
        }

        public Exercise GetExercise()
        {
            return exercise;
        }

        public List<Reward> CompleteExercise()
        {
            SetExerciseCompletion(TypeOfExerciseCompletion.Done);
            return exercise.exerciseRewards;
        }

        public TypeOfExerciseCompletion GetExerciseCompletion()
        {
            return currentExerciseCompletion;
        }
    }
}
