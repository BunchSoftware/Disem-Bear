using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBeforeMailSettings : MonoBehaviour
{
    private Dropdown dropdown;
    private NotificationManager notificationManager;
    [SerializeField] private List<TimeBefore> timeBeforeMailSettings;

    public void Init(NotificationManager notificationManager)
    {
        this.notificationManager = notificationManager;
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> textOptions = new List<string>();
        for (int i = 0; i < timeBeforeMailSettings.Count; i++)
        {
            textOptions.Add($"{timeBeforeMailSettings[i].nameSetting}");
        }
        dropdown.AddOptions(textOptions);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) =>
        {
            notificationManager.timeBeforeMail = timeBeforeMailSettings[value];
            PlayerPrefs.SetInt("TimeBeforeMailSettings", value);
        });

        if (PlayerPrefs.HasKey("TimeBeforeMailSettings"))
        {
            int index = PlayerPrefs.GetInt("TimeBeforeMailSettings", 0);
            if (index >= 0 && index <= timeBeforeMailSettings.Count)
            {
                notificationManager.timeBeforeMail = timeBeforeMailSettings[index];
                dropdown.value = index;
            }
            else
            {
                TimeBefore timeBefore = new();
                timeBefore.nameSetting = "1 минута";
                timeBefore.timeBefore = 60f;
                notificationManager.timeBeforeMail = timeBefore;
            }
        }
        else
        {
            TimeBefore timeBefore = new();
            timeBefore.nameSetting = "1 минута";
            timeBefore.timeBefore = 60f;
            notificationManager.timeBeforeMail = timeBefore;
            dropdown.value = 0;
        }
    }
}


