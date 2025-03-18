using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet.Exercise;
using UnityEngine;
using UnityEngine.UI;
public class ResourceGUI : MonoBehaviour
{
    [Header("�������� �������� ���������� ����")]
    [SerializeField] private Text headerText;
    [SerializeField] private Text countResourceText;
    [SerializeField] private Image avatar;
    private ResourceData resourceData;

    public void UpdateData(ResourceData resourceData)
    {
        this.resourceData = resourceData;
        headerText.text = resourceData.headerResource;
        countResourceText.text = resourceData.countResource.ToString() + " ��.";
        avatar.sprite = resourceData.iconResource;
    }

    public ResourceData GetResourceData()
    {
        return resourceData;
    }
}
