using API;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public interface IUpdateListener
{
    void OnUpdate(float deltaTime);
}

public interface IFixedUpdateListener
{
    void OnFixedUpdate(float fixedDeltaTime);
}

public class GameBootstrap : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerChangeImage playerChangeImage = new();
    [SerializeField] private PlayerMouseMove playerMouseMove;
    [Header("Tutorial")]
    [SerializeField] private TutorialRoot tutorialRoot;
    [Header("Environment")]
    [SerializeField] private EnvironmentRoot environmentRoot;
    [Header("Sound")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private SoundManager musicManager;
    [Header("UI")]
    [SerializeField] private UIGameRoot uiGameRoot;
    [Header("Save System")]
    [SerializeField] private FilePlayer filePlayer;
    [SerializeField] private FileShop fileShop;
    [SerializeField] private APIManager apiManager;

    private SaveManager saveManager = new SaveManager();

    private List<IUpdateListener> updateListeners = new();
    private List<IFixedUpdateListener> fixedUpdateListeners = new();

    private void Awake()
    {
        saveManager.Init(apiManager, filePlayer, fileShop);

        musicManager.Init(this);
        soundManager.Init(this);

        updateListeners.Add(environmentRoot);
        environmentRoot.Init(playerMouseMove);

        tutorialRoot.Init(uiGameRoot.GetDialogManager(), player);

        updateListeners.Add(uiGameRoot);
        uiGameRoot.Init(saveManager);

        playerChangeImage.Init(player.gameObject.GetComponent<Animator>(), player);
        playerMouseMove.OnMove += playerChangeImage.Update;

        updateListeners.Add(playerMouseMove);
        playerMouseMove.Init(player.gameObject.GetComponent<NavMeshAgent>());
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
