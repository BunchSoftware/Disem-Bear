using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PlaneTablet.Window
{
    public class TabBarButton : MonoBehaviour
    {
        [SerializeField] private Color colorSelectedButton;
        [SerializeField] private Color colorDefaultButton;
        [SerializeField] private int indexTab;

        [SerializeField] private List<Image> activateSelectedObjects;
        [SerializeField] private List<Image> colorActivatedSelectedObjetcs;

        private Button button;
        private bool isSelected = false;

        public void Init(Action<int> actionSelectTab)
        {
            for (int i = 0; i < colorActivatedSelectedObjetcs.Count; i++)
            {
                colorActivatedSelectedObjetcs[i].color = colorDefaultButton;
            }
            isSelected = false;

            button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                actionSelectTab.Invoke(indexTab);
            });
        }

        public void SelectButton(bool isSelected)
        {
            this.isSelected = isSelected;

            for (int i = 0; i < colorActivatedSelectedObjetcs.Count; i++)
            {
                colorActivatedSelectedObjetcs[i].color = isSelected ? colorSelectedButton : colorDefaultButton;
            }

            for (int i = 0; i < activateSelectedObjects.Count; i++)
            {
                activateSelectedObjects[i].enabled = isSelected;
            }
        }

        public bool GetIsSelected()
        {
            return isSelected;
        }
    }
}
