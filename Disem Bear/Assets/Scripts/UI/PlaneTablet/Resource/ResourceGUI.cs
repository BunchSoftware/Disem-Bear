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

    public void UpdateData(ResourceData resourceData)
    {
        headerText.text = resourceData.headerResource;
        countResourceText.text = resourceData.countResource.ToString() + " ��.";
        avatar.sprite = resourceData.iconResource;
    }
}
