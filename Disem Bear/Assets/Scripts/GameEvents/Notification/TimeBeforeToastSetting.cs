using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class TimeBeforeToastSetting : MonoBehaviour
{
    private Dropdown dropdown;
    private NotificationManager notificationManager;
    [SerializeField] private List<TimeBefore> timeBeforeToastSettings;

    public void Init(NotificationManager notificationManager)
    {
        this.notificationManager = notificationManager;
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> textOptions = new List<string>();
        for (int i = 0; i < timeBeforeToastSettings.Count; i++)
        {
            textOptions.Add($"{timeBeforeToastSettings[i].nameSetting}");
        }
        dropdown.AddOptions(textOptions);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) =>
        {
            notificationManager.timeBeforeToast = timeBeforeToastSettings[value];
            PlayerPrefs.SetInt("TimeBeforeToastSettings", value);
        });

        if (PlayerPrefs.HasKey("TimeBeforeToastSettings"))
        {
            int index = PlayerPrefs.GetInt("TimeBeforeToastSettings", 0);
            if (index >= 0 && index <= timeBeforeToastSettings.Count)
            {
                notificationManager.timeBeforeToast = timeBeforeToastSettings[index];
                dropdown.value = index;
            }
            else
            {
                TimeBefore timeBefore = new();
                timeBefore.nameSetting = "15 секунд";
                timeBefore.timeBefore = 15f;
                notificationManager.timeBeforeToast = timeBefore;
            }
        }
        else
        {
            TimeBefore timeBefore = new();
            timeBefore.nameSetting = "15 секунд";
            timeBefore.timeBefore = 15f;
            notificationManager.timeBeforeToast = timeBefore;
            dropdown.value = 0;
        }
    }
}

