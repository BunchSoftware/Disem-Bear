using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class FPSSetting
{
    public string nameFPSSetting;
    public int limitFPS;
}

public class FPSSettingsControl : MonoBehaviour
{
    private Dropdown dropdown;
    [SerializeField] private List<FPSSetting> fpsSettings;

    public void Init()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> textOptions = new List<string>();
        for (int i = 0; i < fpsSettings.Count; i++)
        {
            textOptions.Add($"{fpsSettings[i].nameFPSSetting}");
        }
        dropdown.AddOptions(textOptions);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) =>
        {
            Application.targetFrameRate = fpsSettings[value].limitFPS;
            PlayerPrefs.SetInt("FPSLimit", value);
        });

        if (PlayerPrefs.HasKey("FPSLimit"))
        {
            int index = PlayerPrefs.GetInt("FPSLimit", -1);
            if (index >= 0 && index <= fpsSettings.Count)
            {
                Application.targetFrameRate = fpsSettings[index].limitFPS;
                dropdown.value = index;
            }
            else
            {
                Application.targetFrameRate = -1;
            }
        }
        else
        {
            Application.targetFrameRate = -1;
            dropdown.value = 0;
        }
    }
}
