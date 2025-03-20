using External.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet.Shop;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResourceManager : IDisposable
{
    [SerializeField] private GameObject prefabeResourceGUI;
    [SerializeField] private GameObject content;
    [SerializeField] private FileResources fileResources;
    private List<ResourceGUI> resourceGUIs = new List<ResourceGUI>();

    public void Init()
    {
        float width = content.GetComponent<RectTransform>().rect.width;
        Vector2 contentSize = new Vector2((width - content.GetComponent<GridLayoutGroup>().spacing.x * content.GetComponent<GridLayoutGroup>().constraintCount) / content.GetComponent<GridLayoutGroup>().constraintCount, content.GetComponent<GridLayoutGroup>().cellSize.y);
        content.GetComponent<GridLayoutGroup>().cellSize = contentSize;

        if (SaveManager.filePlayer.JSONPlayer.resources.ingradients != null)
        {
            for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.ingradients.Count; i++)
            {
                for (int j = 0; j < fileResources.resources.Count; j++)
                {
                    if (fileResources.resources[j].typeResource == SaveManager.filePlayer.JSONPlayer.resources.ingradients[i].typeIngradient)
                    {
                        ResourceGUI resourceGUI = GameObject.Instantiate(prefabeResourceGUI, content.transform).GetComponent<ResourceGUI>();
                        
                        ResourceData resourceData = new ResourceData();
                        resourceData.headerResource = fileResources.resources[j].headerResource;
                        resourceData.typeResource = fileResources.resources[j].typeResource;
                        resourceData.countResource = SaveManager.filePlayer.JSONPlayer.resources.ingradients[i].countIngradient;
                        resourceData.iconResource = fileResources.resources[j].iconResource;

                        resourceGUI.UpdateData(resourceData);

                        resourceGUIs.Add(resourceGUI);
                    }
                }
            }
        }

        SaveManager.OnUpdatePlayerFile += UpdatePlayerFile;
    }

    private void UpdatePlayerFile()
    {
        for (int i = 0; i < resourceGUIs.Count; i++)
        {
            if (resourceGUIs[i].GetResourceData().typeResource == SaveManager.filePlayer.JSONPlayer.resources.ingradients[i].typeIngradient)
            {
                ResourceData resourceData = new ResourceData();
                resourceData.headerResource = fileResources.resources[i].headerResource;
                resourceData.typeResource = fileResources.resources[i].typeResource;
                resourceData.countResource = SaveManager.filePlayer.JSONPlayer.resources.ingradients[i].countIngradient;
                resourceData.iconResource = fileResources.resources[i].iconResource;

                resourceGUIs[i].UpdateData(resourceData);
            }
        }
    }

    public void Dispose()
    {
        SaveManager.OnUpdatePlayerFile -= UpdatePlayerFile;
    }
}
