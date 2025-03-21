using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class FPSCounterSetting
{
    public string nameFPSCounterSetting;
    public bool enableFPSCounter;
}

public class FPSCounterSettingsController : MonoBehaviour
{
    private Dropdown dropdown;
    private FPSCounter fpsCounter;
    [SerializeField] private List<FPSCounterSetting> fpsCounterSettings;

    public void Init(FPSCounter fPSCounter)
    {
        this.fpsCounter = fPSCounter;

        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> textOptions = new List<string>();
        for (int i = 0; i < fpsCounterSettings.Count; i++)
        {
            textOptions.Add($"{fpsCounterSettings[i].nameFPSCounterSetting}");
        }
        dropdown.AddOptions(textOptions);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) =>
        {
            if (fpsCounterSettings[value].enableFPSCounter)
                fpsCounter.transform.parent.gameObject.SetActive(true);
            else
                fpsCounter.transform.parent.gameObject.SetActive(false);

            PlayerPrefs.SetInt("FPSCounter", value);
        });

        if (PlayerPrefs.HasKey("FPSCounter"))
        {
            int index = PlayerPrefs.GetInt("FPSCounter", 0);
            if (index >= 0 && index <= fpsCounterSettings.Count)
            {
                if (fpsCounterSettings[index].enableFPSCounter)
                    fpsCounter.transform.parent.gameObject.SetActive(true);
                else
                    fpsCounter.transform.parent.gameObject.SetActive(false);
                dropdown.value = index;
            }
            else
                fpsCounter.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            fpsCounter.transform.parent.gameObject.SetActive(false);
            dropdown.value = 0;
        }
    }
}
