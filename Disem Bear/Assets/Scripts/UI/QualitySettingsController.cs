using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace UI
{

    [Serializable]
    public class QualitySetting
    {
        public string nameQuality;
        public int indexQuality;
    }

    [RequireComponent(typeof(Dropdown))]
    public class QualitySettingsController : MonoBehaviour
    {
        private Dropdown dropdown;
        [SerializeField] private List<QualitySetting> qualitySettings;

        public void Init()
        {
            dropdown = GetComponent<Dropdown>();
            dropdown.ClearOptions();
            List<string> textOptions = new List<string>();
            for (int i = 0; i < qualitySettings.Count; i++)
            {
                textOptions.Add($"{qualitySettings[i].nameQuality}");
            }
            dropdown.AddOptions(textOptions);
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener((value) =>
            {
                QualitySettings.SetQualityLevel(qualitySettings[value].indexQuality);
                PlayerPrefs.SetInt("QualitySetting", value);
            });

            if (PlayerPrefs.HasKey("QualitySetting"))
            {
                int index = PlayerPrefs.GetInt("QualitySetting", 0);
                if (index >= 0 && index <= qualitySettings.Count)
                {
                    QualitySettings.SetQualityLevel(qualitySettings[index].indexQuality);
                    dropdown.value = index;
                }
                else
                {
                    QualitySettings.SetQualityLevel(QualitySettings.count);
                }
            }
            else
            {
                QualitySettings.SetQualityLevel(0);
                dropdown.value = 0;
            }
        }
    }
}
