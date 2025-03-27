using Game.Environment;
using Game.LDialog;
using UI;
using UnityEngine;

public class MenuBootstrap : Bootstrap
{
    [Header("UI")]
    [SerializeField] private UIMenuRoot uiGameRoot;
    [SerializeField] private UGCManager ugcManager;
    [SerializeField] private PlayerInput playerInput = new();

    private void Awake()
    {
        Time.timeScale = TimeScale;
        Init();

        updateListeners.Add(uiGameRoot);
        uiGameRoot.Init(soundManager);

        ugcManager.Init();

        updateListeners.Add(playerInput);
        Debug.Log("MenuBootstrap: Успешно иницилизировал все части игры");
    }

    public void OpenDirectoryMod()
    {
        ugcManager.OpenDirectoryMod();
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
