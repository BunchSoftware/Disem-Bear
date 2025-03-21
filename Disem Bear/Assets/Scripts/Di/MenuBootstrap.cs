using Game.Environment;
using Game.LDialog;
using UI;
using UnityEngine;

public class MenuBootstrap : Bootstrap
{
    [Header("UI")]
    [SerializeField] private UIMenuRoot uiGameRoot;
    [SerializeField] private PlayerInput playerInput = new();

    private void Awake()
    {
        Time.timeScale = TimeScale;
        Init();

        updateListeners.Add(uiGameRoot);
        uiGameRoot.Init(soundManager);

        updateListeners.Add(playerInput);
    }
}
