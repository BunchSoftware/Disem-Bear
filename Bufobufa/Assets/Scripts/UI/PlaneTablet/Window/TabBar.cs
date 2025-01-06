using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Window
{
    public class TabBar : MonoBehaviour
    {
        [SerializeField] private GameObject buttons;

        private List<TabBarButton> tabBarButtons = new List<TabBarButton>();

        public void Init(TabManager tabManager)
        {
            tabBarButtons.Clear();
            if (buttons != null)
            {
                for (int i = 0; i < buttons.transform.childCount; i++)
                {
                    TabBarButton button;
                    if (buttons.transform.GetChild(i).TryGetComponent<TabBarButton>(out button))
                    {
                        button.Init((index) =>
                        {
                            tabManager.SetCurrentIndexTab(index);
                            button.SelectButton(true);
                        });
                        tabBarButtons.Add(button);
                    }
                }
            }
            else
                throw new System.Exception("Ошибка ! Назначьте в инспекторе Buttons");
        }

        public List<TabBarButton> GetTabBarButtons()
        {
            return tabBarButtons;
        }
    }
}
