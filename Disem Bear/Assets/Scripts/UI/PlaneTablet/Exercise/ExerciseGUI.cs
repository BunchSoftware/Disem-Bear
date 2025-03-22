
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
        [SerializeField] private GameObject prefabItem;
        [SerializeField] private RectTransform description;
        [SerializeField] private Button exerciseButton;
        [SerializeField] private Image checkMark;

        [Header("������ ���������� �������")]
        [SerializeField] private Button runButton;
        [SerializeField] private Button executionButton;
        [SerializeField] private Button doneButton;

        [Header("�������� �������� �������")]
        [SerializeField] private Text headerText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Image avatar;
        [SerializeField] private Image arrowUp;
        [SerializeField] private Image arrowDown;
        [SerializeField] private Text headerExerciseItems;
        [SerializeField] private GameObject contentRewards;
        [SerializeField] private GameObject contentItem;

        [Header("�������� ������ � �������")]
        [SerializeField] private Image background;
        [SerializeField] private Color colorNotDoneExerciseBackground;
        [SerializeField] private Color colorDoneExerciseBackground;
        [SerializeField] private Color colorRunExerciseBackground;

        [Header("������ ���������� ������")]
        [SerializeField] private Button resetButton;

        private TypeOfExerciseCompletion currentExerciseCompletion = TypeOfExerciseCompletion.NotDone;
        private bool isExpandExercise = false;
        private Exercise exercise;
        private ExerciseManager exerciseManager;
        private List<ExerciseRewardGUI> exerciseRewardGUIs = new List<ExerciseRewardGUI>();
        private List<ExerciseItemGUI> exerciseItemGUIs = new List<ExerciseItemGUI>();
        private int indexExercise = 0;

        public void Init(ExerciseManager exerciseManager, Action<ExerciseGUI, bool> ActionExercise, Exercise exercise, int indexExercise)
        {
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
                SetExerciseCompletion(TypeOfExerciseCompletion.Run);
                ActionExercise.Invoke(this, false);
                if (!SaveManager.playerDatabase.JSONPlayer.resources.exercises[indexExercise].isGetExerciesItems)
                    exerciseManager.GiveExerciseItem(exercise);

                SaveManager.playerDatabase.JSONPlayer.resources.exercises[indexExercise].isGetExerciesItems = true;
                SaveManager.UpdatePlayerDatabase();
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
            gameObject.SetActive(exercise.isVisible);
            runButton.gameObject.SetActive(!exercise.isMail);

            if (exercise.isRandom && !exercise.isMail)
                resetButton.gameObject.SetActive(true);
            else
                resetButton.gameObject.SetActive(false);

            if (exerciseRewardGUIs.Count == 0)
            {
                for (int i = 0; i < exercise.exerciseRewards.Count; i++)
                {
                    ExerciseRewardGUI exerciseRewardGUI = Instantiate(prefabReward, contentRewards.transform).GetComponent<ExerciseRewardGUI>();
                    exerciseRewardGUI.countRewardText.text = $"{exercise.exerciseRewards[i].countReward}x";
                    exerciseRewardGUI.avatarReward.sprite = exercise.exerciseRewards[i].avatarReward;
                    exerciseRewardGUIs.Add(exerciseRewardGUI);
                }

                for (int i = 0; i < exercise.exerciseItems.Count; i++)
                {
                    ExerciseItemGUI exerciseItemGUI = Instantiate(prefabItem, contentItem.transform).GetComponent<ExerciseItemGUI>();
                    exerciseItemGUI.countItemText.text = $"{exercise.exerciseItems[i].countItem}x";
                    exerciseItemGUI.avatarItem.sprite = exercise.exerciseItems[i].avatarItem;
                    exerciseItemGUIs.Add(exerciseItemGUI);
                }

                if (exercise.exerciseItems.Count == 0)
                    headerExerciseItems.gameObject.SetActive(false);
                else
                    headerExerciseItems.gameObject.SetActive(true);
            }
            else
            {
                for (int i = 0; i < exerciseRewardGUIs.Count; i++)
                {
                    Destroy(exerciseRewardGUIs[i].gameObject);
                }
                exerciseRewardGUIs.Clear();

                for (int i = 0; i < exerciseItemGUIs.Count; i++)
                {
                    Destroy(exerciseItemGUIs[i].gameObject);
                }
                exerciseItemGUIs.Clear();

                if (exercise.exerciseItems.Count == 0)
                    headerExerciseItems.gameObject.SetActive(false);
                else
                    headerExerciseItems.gameObject.SetActive(true);

                for (int i = 0; i < exercise.exerciseRewards.Count; i++)
                {
                    ExerciseRewardGUI exerciseRewardGUI = Instantiate(prefabReward, contentRewards.transform).GetComponent<ExerciseRewardGUI>();
                    exerciseRewardGUI.countRewardText.text = $"{exercise.exerciseRewards[i].countReward}x";
                    exerciseRewardGUI.avatarReward.sprite = exercise.exerciseRewards[i].avatarReward;
                    exerciseRewardGUIs.Add(exerciseRewardGUI);
                }
                for (int i = 0; i < exercise.exerciseItems.Count; i++)
                {
                    ExerciseItemGUI exerciseItemGUI = Instantiate(prefabItem, contentItem.transform).GetComponent<ExerciseItemGUI>();
                    exerciseItemGUI.countItemText.text = $"{exercise.exerciseItems[i].countItem}x";
                    exerciseItemGUI.avatarItem.sprite = exercise.exerciseItems[i].avatarItem;
                    exerciseItemGUIs.Add(exerciseItemGUI);
                }
            }


            headerText.text = exercise.header;
            descriptionText.text = exercise.description;
            avatar.sprite = exercise.avatar;
            avatar.preserveAspect = true;
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
                throw new System.Exception("������ ! �������� ������ Description");
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
                        checkMark.gameObject.SetActive(true);
                        exercise.isVisible = false;

                        for (int i = 0; i < exercise.newExercises.Count; i++)
                        {
                            exerciseManager.AddExercise(exercise.newExercises[i]);
                        }

                        UpdateData(exercise);

                        for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.exercises.Count; i++)
                        {
                            if (SaveManager.playerDatabase.JSONPlayer.resources.exercises[i].nameExercise == exercise.nameExercise)
                                SaveManager.playerDatabase.JSONPlayer.resources.exercises[i].isVisible = false;
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

        public List<Reward> CompleteExercise(string messageCondition)
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
