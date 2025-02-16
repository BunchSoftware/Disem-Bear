using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class FullScreenSetting
{
    public string nameFullScreen;
    public FullScreenMode fullScreenMode;
}
[RequireComponent(typeof(Dropdown))]
public class FullScreenSettingsController : MonoBehaviour
{
    private Dropdown dropdown;
    [SerializeField] private List<FullScreenSetting> fullScreenSettings;

    public void Init()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> textOptions = new List<string>();
        for (int i = 0; i < fullScreenSettings.Count; i++)
        {
            textOptions.Add($"{fullScreenSettings[i].nameFullScreen}");
        }
        dropdown.AddOptions(textOptions);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) =>
        {
            Screen.SetResolution(Screen.width, Screen.height, fullScreenSettings[value].fullScreenMode);
            PlayerPrefs.SetInt("FullScreen", value);
        });

        if (PlayerPrefs.HasKey("FullScreen"))
        {
            int index = PlayerPrefs.GetInt("FullScreen", 0);
            if (index >= 0 && index <= fullScreenSettings.Count)
            {
                Screen.SetResolution(Screen.width, Screen.height, fullScreenSettings[index].fullScreenMode);
                dropdown.value = index;
            }
            else
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
            }
        }
        else
        {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
            dropdown.value = 0;
        }
    }
}
