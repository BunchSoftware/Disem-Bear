
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
        [SerializeField] private RectTransform description;
        [SerializeField] private Button exerciseButton;
        [SerializeField] private Image checkMark;

        [Header("Кнопка посылки")]
        [SerializeField] private Button givePackage;

        [Header("Кнопки выполнения задания")]
        [SerializeField] private Button runButton;
        [SerializeField] private Button executionButton;
        [SerializeField] private Button doneButton;

        [Header("Основные элементы заданий")]
        [SerializeField] private Text headerText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Image avatar;

        [Header("Цветовой бортик с цветами")]
        [SerializeField] private Image background;
        [SerializeField] private Color colorNotDoneExerciseBackground;
        [SerializeField] private Color colorDoneExerciseBackground;
        [SerializeField] private Color colorRunExerciseBackground;

        private TypeOfExerciseCompletion currentExerciseCompletion = TypeOfExerciseCompletion.NotDone;
        private bool isExpandExercise = false;
        private Exercise exercise;
        private List<ExerciseRewardGUI> exerciseRewardGUIs = new List<ExerciseRewardGUI>();

        public bool isGetPackage = false;
        private int indexExercise = 0;

        public void Init(ExerciseManager exerciseManager, Action<ExerciseGUI, bool> ActionExercise, Exercise exercise, int indexExercise)
        {
            this.indexExercise = indexExercise;

            SetExerciseCompletion(SaveManager.filePlayer.JSONPlayer.resources.exercises[this.indexExercise].typeOfExerciseCompletion);
            isGetPackage = SaveManager.filePlayer.JSONPlayer.resources.exercises[this.indexExercise].isGetPackage;

            if (isGetPackage)
                givePackage.interactable = false;
            else
                givePackage.interactable = true;

            executionButton.onClick.RemoveAllListeners();

            exerciseButton.onClick.AddListener(() =>
            {
                ActionExercise.Invoke(this, true);

                if (isExpandExercise)
                    ExpandExercise(false);
                else
                    ExpandExercise(true);
            });

            runButton.onClick.RemoveAllListeners();

            runButton.onClick.AddListener(() =>
            {
                SetExerciseCompletion(TypeOfExerciseCompletion.Run);
                ActionExercise.Invoke(this, false);
            });

            givePackage.onClick.RemoveAllListeners();
            givePackage.onClick.AddListener(() =>
            {
                exerciseManager.GivePackage(exercise);
                givePackage.interactable = false;

                SaveManager.filePlayer.JSONPlayer.resources.exercises[indexExercise].isGetPackage = true;
            });

            this.exercise = exercise;

            headerText.text = exercise.header;

            for (int i = 0; i < exercise.exerciseRewards.Count; i++)
            {
                ExerciseRewardGUI exerciseRewardGUI = Instantiate(prefabReward, exerciseButton.transform).GetComponent<ExerciseRewardGUI>();
                exerciseRewardGUI.countRewardText.text = $"{exercise.exerciseRewards[i].countReward}x";
                exerciseRewardGUI.avatarReward.sprite = exercise.exerciseRewards[i].avatarReward;

                exerciseRewardGUIs.Add(exerciseRewardGUI);
            }

            descriptionText.text = exercise.description;
            avatar.sprite = exercise.avatar;
            avatar.preserveAspect = true;
            avatar.preserveAspect = true;
        }

        public void ExpandExercise(bool isExpandExercise)
        {
            this.isExpandExercise = isExpandExercise;
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
            SaveManager.filePlayer.JSONPlayer.resources.exercises[indexExercise].typeOfExerciseCompletion = exerciseCompletion;

            switch (exerciseCompletion)
            {
                case TypeOfExerciseCompletion.NotDone:
                    {
                        background.color = colorNotDoneExerciseBackground;
                        runButton.gameObject.SetActive(true);
                        doneButton.gameObject.SetActive(false);
                        executionButton.gameObject.SetActive(false);
                        checkMark.gameObject.SetActive(false);
                        break;
                    }
                case TypeOfExerciseCompletion.Run:
                    {
                        background.color = colorRunExerciseBackground;
                        runButton.gameObject.SetActive(false);
                        doneButton.gameObject.SetActive(false);
                        executionButton.gameObject.SetActive(true);
                        checkMark.gameObject.SetActive(false);
                        break;
                    }
                case TypeOfExerciseCompletion.Done:
                    {
                        background.color = colorDoneExerciseBackground;
                        runButton.gameObject.SetActive(false);
                        doneButton.gameObject.SetActive(true);
                        executionButton.gameObject.SetActive(false);
                        checkMark.gameObject.SetActive(true);
                        break;
                    }
            }
        }

        public Exercise GetExercise()
        {
            return exercise;
        }

        public List<Reward> DoneExercise(string messageCondition)
        {
            List<Reward> exerciseRewards = exercise.DoneExercise(messageCondition);
            SetExerciseCompletion(TypeOfExerciseCompletion.Done);
            return exerciseRewards;
        }

        public TypeOfExerciseCompletion GetExerciseCompletion()
        {
            return currentExerciseCompletion;
        }
    }
}
