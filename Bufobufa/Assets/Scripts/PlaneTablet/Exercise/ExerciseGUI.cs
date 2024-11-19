
using System;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Кнопки выполнения задания")]
    [SerializeField] private Button runButton;
    [SerializeField] private Button executionButton;
    [SerializeField] private Button doneButton;

    [Header("Основные элементы заданий")]
    [SerializeField] private Text headerText;
    [SerializeField] private Text rewardText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image avatar;

    [Header("Цветовой бортик с цветами")]
    [SerializeField] private Image background;
    [SerializeField] private Color colorNotDoneExerciseBackground;
    [SerializeField] private Color colorDoneExerciseBackground;
    [SerializeField] private Color colorRunExerciseBackground;

    private TypeOfExerciseCompletion currentExerciseCompletion = TypeOfExerciseCompletion.NotDone;
    private bool isExpandExercise = false;
    private RectTransform rectTransform;
    private Exercise exercise;

    public void Init(Action<ExerciseGUI, bool> ActionExercise, JSONExercise JSONExercise, Exercise exercise)
    {
        rectTransform = GetComponent<RectTransform>();
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
        });

        this.exercise = exercise;   

        headerText.text = JSONExercise.header;
        rewardText.text = JSONExercise.reward;
        descriptionText.text = JSONExercise.description;
        avatar.sprite = Resources.Load<Sprite>("Avatars/" + JSONExercise.pathToAvatar);
    }

    public void ExpandExercise(bool isExpandExercise)
    {
        this.isExpandExercise = isExpandExercise;
        if (description != null)
        {
            description.gameObject.SetActive(isExpandExercise);
            if (isExpandExercise)
            {
                RectTransform rectTransformButton = exerciseButton.gameObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, description.sizeDelta.y + rectTransformButton.sizeDelta.y);
            }
            else
            {
                RectTransform rectTransformButton = exerciseButton.gameObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransformButton.sizeDelta.y);
            }
        }
        else
            throw new System.Exception("Ошибка ! Добавьте обьект Description");
    }

    public void SetExerciseCompletion(TypeOfExerciseCompletion exerciseCompletion)
    {
        currentExerciseCompletion = exerciseCompletion;

        switch(exerciseCompletion)
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

    public ExerciseReward DoneExercise()
    {
        SetExerciseCompletion(TypeOfExerciseCompletion.Done);
        return exercise.DoneExercise();
    }

    public TypeOfExerciseCompletion GetExerciseCompletion()
    {
        return currentExerciseCompletion;
    }
}
