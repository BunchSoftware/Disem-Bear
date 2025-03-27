using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Dropdown))]
public class NeedSendMailSettings : MonoBehaviour
{
    private Dropdown dropdown;
    private NotificationManager notificationManager;
    [SerializeField] private List<NeedMailSetting> needMailSettings;

    public void Init(NotificationManager notificationManager)
    {
        this.notificationManager = notificationManager;
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> textOptions = new List<string>();
        for (int i = 0; i < needMailSettings.Count; i++)
        {
            textOptions.Add($"{needMailSettings[i].nameSetting}");
        }
        dropdown.AddOptions(textOptions);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) =>
        {
            notificationManager.needMail = needMailSettings[value].state;
            PlayerPrefs.SetInt("NeedMailSetting", value);
        });

        if (PlayerPrefs.HasKey("NeedMailSetting"))
        {
            int index = PlayerPrefs.GetInt("NeedMailSetting", 0);
            if (index >= 0 && index <= needMailSettings.Count)
            {
                notificationManager.needMail = needMailSettings[index].state;
                dropdown.value = index;
            }
            else
            {
                notificationManager.needMail = true;
            }
        }
        else
        {
            notificationManager.needMail = true;
            dropdown.value = 0;
        }
    }
}

[Serializable]
public class NeedMailSetting
{
    public string nameSetting;
    public bool state;
}

