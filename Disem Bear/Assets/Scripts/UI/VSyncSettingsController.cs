using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class VSyncSetting
{
    public string nameVSyncSetting;
    public bool enableVSync;
}
public class VSyncSettingsController : MonoBehaviour
{
    private Dropdown dropdown;
    [SerializeField] private List<VSyncSetting> vSyncSettings;
    [SerializeField] private FPSSettingsControl fpsSettingsControl;

    public void Init()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> textOptions = new List<string>();
        for (int i = 0; i < vSyncSettings.Count; i++)
        {
            textOptions.Add($"{vSyncSettings[i].nameVSyncSetting}");
        }
        dropdown.AddOptions(textOptions);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) =>
        {
            if (vSyncSettings[value].enableVSync)
            {
                QualitySettings.vSyncCount = 1;
                fpsSettingsControl.GetComponent<Dropdown>().interactable = false;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                fpsSettingsControl.GetComponent<Dropdown>().interactable = true;
            }

            PlayerPrefs.SetInt("VSync", value);
        });

        if (PlayerPrefs.HasKey("VSync"))
        {
            int index = PlayerPrefs.GetInt("VSync", 0);
            if (index >= 0 && index <= vSyncSettings.Count)
            {
                if (vSyncSettings[index].enableVSync)
                {
                    QualitySettings.vSyncCount = 1;
                    fpsSettingsControl.GetComponent<Dropdown>().interactable = false;
                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                    fpsSettingsControl.GetComponent<Dropdown>().interactable = true;
                }
                dropdown.value = index;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                fpsSettingsControl.GetComponent<Dropdown>().interactable = true;
            }
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            fpsSettingsControl.GetComponent<Dropdown>().interactable = true;
            dropdown.value = 0;
        }
    }
}
