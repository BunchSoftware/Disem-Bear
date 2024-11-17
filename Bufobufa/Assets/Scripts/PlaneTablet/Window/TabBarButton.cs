using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabBarButton : MonoBehaviour
{
    [SerializeField] private Color colorSelectedButton;
    [SerializeField] private Color colorDefaultButton;
    [SerializeField] private int indexTab;

    private Button button;
    private Image image;
    private bool isSelected = false;

    public void Init(Action<int> actionSelectTab)
    {
        image = GetComponent<Image>();
        image.color = colorDefaultButton;

        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            actionSelectTab.Invoke(indexTab);
        });
    }

    public void SelectButton(bool isSelected)
    {
        this.isSelected = isSelected;

        if (isSelected)
            image.color = colorSelectedButton;
        else
            image.color = colorDefaultButton;
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }
}
