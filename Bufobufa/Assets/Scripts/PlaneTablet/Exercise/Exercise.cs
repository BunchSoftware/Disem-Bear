
using System;
using UnityEngine;
using UnityEngine.UI;

public class Exercise : MonoBehaviour
{
    [SerializeField] private RectTransform description;
    [SerializeField] private Button exerciseButton;
    private bool isExpandExercise = false;
    private RectTransform rectTransform;

    public void Init(Action<Exercise> ExpandAllExercise)
    {
        rectTransform = GetComponent<RectTransform>();
        exerciseButton.onClick.AddListener(() =>
        {
            ExpandAllExercise.Invoke(this);

            if (isExpandExercise)
                ExpandExercise(false);
            else
                ExpandExercise(true);
        });
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
}
