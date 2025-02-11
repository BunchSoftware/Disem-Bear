using External.API;
using External.DI;
using External.Storage;
using Game.Environment.Fridge;
using Game.Environment;
using Game.LPlayer;
using Game.Music;
using Game.Tutorial;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.AI;

public class MenuBootstrap : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private SoundManager musicManager;
    [Header("UI")]
    [SerializeField] private UIMenuRoot uiGameRoot;
    [Header("Save System")]
    [SerializeField] private FilePlayer filePlayer;
    [SerializeField] private APIManager apiManager;

    [SerializeField] private PlayerInput playerInput = new();

    private SaveManager saveManager = new SaveManager();

    private List<IUpdateListener> updateListeners = new();
    private List<IFixedUpdateListener> fixedUpdateListeners = new();

    private void Awake()
    {
        saveManager.Init(apiManager, filePlayer);

        musicManager.Init(this);
        soundManager.Init(this);

        updateListeners.Add(uiGameRoot);
        uiGameRoot.Init(saveManager, soundManager);

        updateListeners.Add(playerInput);
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        for (int i = 0, count = updateListeners.Count; i < count; i++)
        {
            var listener = updateListeners[i];
            listener.OnUpdate(deltaTime);
        }
    }

    private void FixedUpdate()
    {
        var fixedDeltaTime = Time.fixedDeltaTime;
        for (int i = 0, count = fixedUpdateListeners.Count; i < count; i++)
        {
            var listener = fixedUpdateListeners[i];
            listener.OnFixedUpdate(fixedDeltaTime);
        }
    }

    public void AddUpdateListener(IUpdateListener updateListener)
    {
        updateListeners.Add(updateListener);
    }

    public void RemoveUpdateListener(IUpdateListener updateListener)
    {
        updateListeners.Remove(updateListener);
    }

    public void AddFixedUpdateListener(IFixedUpdateListener fixedUpdateListener)
    {
        fixedUpdateListeners.Add(fixedUpdateListener);
    }

    public void RemoveFixedUpdateListener(IFixedUpdateListener fixedUpdateListener)
    {
        fixedUpdateListeners.Remove(fixedUpdateListener);
    }
}
