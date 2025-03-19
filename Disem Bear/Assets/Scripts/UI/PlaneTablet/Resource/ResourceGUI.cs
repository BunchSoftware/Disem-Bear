using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet.Exercise;
using UnityEngine;
using UnityEngine.UI;
public class ResourceGUI : MonoBehaviour
{
    [Header("Основные элементы ресурсного меню")]
    [SerializeField] private Text countResourceText;
    [SerializeField] private Image avatar;
    [SerializeField] private ToolTipTrigger toolTipTrigger;
    private ResourceData resourceData;

    public void UpdateData(ResourceData resourceData)
    {
        this.resourceData = resourceData;
        toolTipTrigger.message = resourceData.headerResource;
        countResourceText.text = resourceData.countResource.ToString() + " шт.";
        avatar.sprite = resourceData.iconResource;
    }

    public ResourceData GetResourceData()
    {
        return resourceData;
    }
}
