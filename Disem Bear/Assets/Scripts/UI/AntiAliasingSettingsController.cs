using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


[Serializable]
public class AntiAliasingSetting
{
    public string nameAntiAliasing;
    public int antiAliasing;
}
public class AntiAliasingSettingsController : MonoBehaviour
{
    private Dropdown dropdown;
    [SerializeField] private List<AntiAliasingSetting> antiAliasingSettings;

    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> textOptions = new List<string>();
        for (int i = 0; i < antiAliasingSettings.Count; i++)
        {
            textOptions.Add($"{antiAliasingSettings[i].nameAntiAliasing}");
        }
        dropdown.AddOptions(textOptions);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) =>
        {
            Screen.SetMSAASamples(antiAliasingSettings[value].antiAliasing);
            PlayerPrefs.SetInt("AntiAliasing", value);
        });

        if (PlayerPrefs.HasKey("AntiAliasing"))
        {
            int index = PlayerPrefs.GetInt("AntiAliasing", antiAliasingSettings.Count);
            if (index >= 0 && index <= antiAliasingSettings.Count)
            {
                Screen.SetMSAASamples(antiAliasingSettings[index].antiAliasing);
                dropdown.value = index;
            }
            else
            {
                Screen.SetMSAASamples(8);
            }
        }
        else
        {
            Screen.SetMSAASamples(8);
            dropdown.value = 0;
        }
    }
}
