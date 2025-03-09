using External.API;
using External.Storage;
using Game.Environment;
using Game.Environment.Fridge;
using Game.LPlayer;
using Game.Music;
using Game.Tutorial;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace External.DI
{
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
        [SerializeField] private bool isTest = true;
        [SerializeField] private FilePlayer defaultFilePlayer;
        [SerializeField] private FileShop defaultFileShop;
        [SerializeField] private FilePlayer filePlayer;
        [SerializeField] private FileShop fileShop;
        [SerializeField] private APIManager apiManager;

        [SerializeField] private PlayerInput playerInput = new();

        private SaveManager saveManager = new SaveManager();

        private List<IUpdateListener> updateListeners = new();
        private List<IFixedUpdateListener> fixedUpdateListeners = new();

        private void Awake()
        {
            PlayerPrefs.DeleteAll();
            #region Init
            if (!player)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� Player");
                return;
            }

            if (!tutorialRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� TutorialRoot");
                return;
            }

            if (!environmentRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� EnvironmentRoot");
                return;
            }

            if (!uiGameRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� UIGameRoot");
                return;
            }

            if (!filePlayer)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� FilePlayer");
                return;
            }

            if (!fileShop)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� FileShop");
                return;
            }
            #endregion

            apiManager.Init();

            saveManager.Init(apiManager, isTest, filePlayer, fileShop, defaultFilePlayer, defaultFileShop);

            musicManager.Init(this);
            soundManager.Init(this);

            updateListeners.Add(environmentRoot);
            environmentRoot.Init(player, playerMouseMove, saveManager, soundManager);

            updateListeners.Add(tutorialRoot);
            tutorialRoot.Init(uiGameRoot.GetDialogManager(), player, saveManager);

            updateListeners.Add(uiGameRoot);
            uiGameRoot.Init(saveManager);

            playerChangeImage.Init(player.gameObject.GetComponent<Animator>(), player);
            playerMouseMove.OnMove += playerChangeImage.Update;

            updateListeners.Add(playerMouseMove);
            playerMouseMove.Init(player.gameObject.GetComponent<NavMeshAgent>());

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
}
