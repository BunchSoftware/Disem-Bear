
using External.Storage;
using System;
using UnityEngine;
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
        [SerializeField] private Image avatarReward;
        [SerializeField] private Text countRewardText;
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

        public bool isGetPackage = false;
        private int indexExercise = 0;
        private SaveManager saveManager;

        public void Init(ExerciseManager exerciseManager, Action<ExerciseGUI, bool> ActionExercise, Exercise exercise, int indexExercise, SaveManager saveManager)
        {
            this.indexExercise = indexExercise;
            this.saveManager = saveManager;

            SetExerciseCompletion(this.saveManager.filePlayer.JSONPlayer.resources.exerciseSaves[this.indexExercise].typeOfExerciseCompletion);
            isGetPackage = this.saveManager.filePlayer.JSONPlayer.resources.exerciseSaves[this.indexExercise].isGetPackage;

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

                saveManager.filePlayer.JSONPlayer.resources.exerciseSaves[indexExercise].isGetPackage = true;
                saveManager.UpdatePlayerFile();
            });

            this.exercise = exercise;

            headerText.text = exercise.header;
            countRewardText.text = $"{exercise.exerciseReward.countReward}x";
            descriptionText.text = exercise.description;
            avatar.sprite = exercise.avatar;
            avatar.preserveAspect = true;
            avatarReward.sprite = exercise.exerciseReward.avatarReward;
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
            saveManager.filePlayer.JSONPlayer.resources.exerciseSaves[indexExercise].typeOfExerciseCompletion = exerciseCompletion;
            saveManager.UpdatePlayerFile();

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

        public ExerciseReward DoneExercise(string messageCondition)
        {
            ExerciseReward exerciseReward = exercise.DoneExercise(messageCondition);
            SetExerciseCompletion(TypeOfExerciseCompletion.Done);
            return exerciseReward;
        }

        public TypeOfExerciseCompletion GetExerciseCompletion()
        {
            return currentExerciseCompletion;
        }
    }
}
