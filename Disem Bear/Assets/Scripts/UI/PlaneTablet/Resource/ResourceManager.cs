using External.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet.Shop;
using UnityEngine;

[Serializable]
public class ResourceManager
{
    [SerializeField] private GameObject prefabeResourceGUI;
    [SerializeField] private GameObject content;
    [SerializeField] private FileResources fileResources;
    private List<ResourceGUI> resourceGUIs = new List<ResourceGUI>();

    public void Init()
    {
        if (SaveManager.filePlayer.JSONPlayer.resources.ingradients != null)
        {
            for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.ingradients.Count; i++)
            {
                for (int j = 0; j < fileResources.resources.Count; j++)
                {
                    Debug.Log(SaveManager.filePlayer.JSONPlayer.resources.ingradients[i].typeIngradient);
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
    }
}
