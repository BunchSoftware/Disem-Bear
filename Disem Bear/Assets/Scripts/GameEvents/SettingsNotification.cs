using System.Collections;
using System.Collections.Generic;
using settingNotification;
using UnityEngine;

public class SettingsNotification : MonoBehaviour
{
    [SerializeField] private NotificationManager notificationManager;
    [SerializeField] private NeedSendToastSettings needSendToastSettings;
    [SerializeField] private TimeBeforeToastSetting timeBeforeToastSetting;
    [SerializeField] private NeedSendMailSettings needSendMailSettings;
    [SerializeField] private TimeBeforeMailSettings timeBeforeMailSetting;

    public void Init()
    {
        needSendToastSettings.Init(notificationManager);
        timeBeforeToastSetting.Init(notificationManager);
        needSendMailSettings.Init(notificationManager);
        timeBeforeMailSetting.Init(notificationManager);
    }
}
