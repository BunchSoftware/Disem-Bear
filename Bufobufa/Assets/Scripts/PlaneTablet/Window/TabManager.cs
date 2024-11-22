using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    [SerializeField] private TabBar tabBar;
    [SerializeField] private GameObject tabsContent;

    private List<Tab> tabs = new List<Tab>();
    private List<TabBarButton> tabBarButtons = new List<TabBarButton>();
    private int currentIndexTab;

    private void OnEnable()
    {
        tabBar.Init(this);

        tabBarButtons.Clear();
        tabs.Clear();

        for (int i = 0; i < tabsContent.transform.childCount; i++)
        {
            Tab tab;
            if (tabsContent.transform.GetChild(i).TryGetComponent<Tab>(out tab))
            {
                if(tab.indexTab == 0)
                {
                    tab.SelectTab(true);
                    tabs.Add(tab);
                    continue;
                }
                tab.SelectTab(false);
                tabs.Add(tab);
            };
        }
        tabBarButtons = tabBar.GetTabBarButtons();

        if (tabBarButtons.Count >= 1)
            tabBarButtons[0].SelectButton(true);
    }

    public void SetCurrentIndexTab(int index)
    {
        currentIndexTab = index;

        for (int i = 0; i < tabBarButtons.Count; i++)
        {
            tabBarButtons[i].SelectButton(false);
        }

        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].SelectTab(false);
        }

        if (index >= 0 && index <= tabs.Count)
            tabs[index].SelectTab(true);
    }

    public int GetCurrentIndexTab()
    {
        return currentIndexTab;
    }
}
