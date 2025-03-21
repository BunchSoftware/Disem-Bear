using UI;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private AntiAliasingSettingsController antiAliasingSettingsController;
    [SerializeField] private FPSSettingsControl fpsSettingsControl;
    [SerializeField] private FullScreenSettingsController fullScreenSettingsController;
    [SerializeField] private QualitySettingsController qualitySettingsController;
    [SerializeField] private ScreenResolutionController screenResolutionController;
    [SerializeField] private VSyncSettingsController vSyncSettingsController;
    [SerializeField] private FPSCounterSettingsController fpsCounterSettingsController;
    [SerializeField] private FPSCounter fpsCounter;

    public void Init()
    {
        antiAliasingSettingsController.Init();
        fpsSettingsControl.Init();
        fullScreenSettingsController.Init();
        qualitySettingsController.Init();
        screenResolutionController.Init();
        vSyncSettingsController.Init();

        if (fpsCounter == null)
            Debug.LogError("Ошибка. Забыли указать FPS Counter");

        fpsCounterSettingsController.Init(fpsCounter);
    }
}
