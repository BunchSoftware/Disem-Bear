using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace settingNotification
{
    [RequireComponent(typeof(Dropdown))]
    public class NeedSendToastSettings: MonoBehaviour
    {
        private Dropdown dropdown;
        private NotificationManager notificationManager;
        [SerializeField] private List<NeedToastSetting> needToastSettings;

        public void Init(NotificationManager notificationManager)
        {
            this.notificationManager = notificationManager;
            dropdown = GetComponent<Dropdown>();
            dropdown.ClearOptions();
            List<string> textOptions = new List<string>();
            for (int i = 0; i < needToastSettings.Count; i++)
            {
                textOptions.Add($"{needToastSettings[i].nameSetting}");
            }
            dropdown.AddOptions(textOptions);
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener((value) =>
            {
                notificationManager.needToast = needToastSettings[value].state;
                PlayerPrefs.SetInt("NeedToastSetting", value);
            });

            if (PlayerPrefs.HasKey("NeedToastSetting"))
            {
                int index = PlayerPrefs.GetInt("NeedToastSetting", 0);
                if (index >= 0 && index <= needToastSettings.Count)
                {
                    notificationManager.needToast = needToastSettings[index].state;
                    dropdown.value = index;
                }
                else
                {
                    notificationManager.needToast = true;
                }
            }
            else
            {
                notificationManager.needToast = true;
                dropdown.value = 0;
            }
        }
    }

    [Serializable]
    public class NeedToastSetting
    {
        public string nameSetting;
        public bool state;
    }
}
