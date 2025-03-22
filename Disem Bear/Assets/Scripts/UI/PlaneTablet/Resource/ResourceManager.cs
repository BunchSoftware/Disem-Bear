using External.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResourceManager : IDisposable
{
    [SerializeField] private GameObject prefabeResourceGUI;
    [SerializeField] private GameObject content;
    [SerializeField] private ResourceDatabase fileResources;
    private List<ResourceGUI> resourceGUIs = new List<ResourceGUI>();

    public void Init()
    {
        UpdateData();

        SaveManager.OnUpdatePlayerFile += UpdateData;

        Debug.Log("ResourceManager: ������� ��������������");
    }

    private void UpdateData()
    {
        for (int i = 0; i < resourceGUIs.Count; i++)
        {
            GameObject.Destroy(resourceGUIs[i].gameObject);
        }
        resourceGUIs.Clear();

        float width = content.GetComponent<RectTransform>().rect.width;
        Vector2 contentSize = new Vector2((width - (content.GetComponent<GridLayoutGroup>().spacing.x * content.GetComponent<GridLayoutGroup>().constraintCount)) / content.GetComponent<GridLayoutGroup>().constraintCount, content.GetComponent<GridLayoutGroup>().cellSize.y);
        content.GetComponent<GridLayoutGroup>().cellSize = contentSize;

        if (SaveManager.playerDatabase.JSONPlayer.resources.ingradients != null)
        {
            for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.ingradients.Count; i++)
            {
                for (int j = 0; j < fileResources.resources.Count; j++)
                {
                    if (fileResources.resources[j].typeResource == SaveManager.playerDatabase.JSONPlayer.resources.ingradients[i].typeIngradient)
                    {
                        ResourceGUI resourceGUI = GameObject.Instantiate(prefabeResourceGUI, content.transform).GetComponent<ResourceGUI>();

                        ResourceData resourceData = new ResourceData();
                        resourceData.headerResource = fileResources.resources[j].headerResource;
                        resourceData.typeResource = fileResources.resources[j].typeResource;
                        resourceData.countResource = SaveManager.playerDatabase.JSONPlayer.resources.ingradients[i].countIngradient;
                        resourceData.iconResource = fileResources.resources[j].iconResource;

                        resourceGUI.UpdateData(resourceData);

                        resourceGUIs.Add(resourceGUI);
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        SaveManager.OnUpdatePlayerFile -= UpdateData;
    }
}
