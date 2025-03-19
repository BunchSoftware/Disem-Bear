using External.API;
using External.Storage;
using Game.Environment;
using Game.Environment.Fridge;
using Game.Environment.Item;
using Game.LPlayer;
using Game.Music;
using Game.Tutorial;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.InputManagerEntry;

namespace External.DI
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private Fade fade;
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
        [SerializeField] private ToastManager toastManager;
        [SerializeField] private FilePrefabsPickUpItems filePrefabsPickUpItems;
        private static FilePrefabsPickUpItems FilePrefabsPickUpItems;
        [Header("Save System")]
        [SerializeField] private FilePlayer defaultFilePlayer;
        [SerializeField] private FileShop defaultFileShop;
        [SerializeField] private FilePlayer filePlayer;
        [SerializeField] private FileShop fileShop;
        [SerializeField] private APIManager apiManager;

        [SerializeField] private PlayerInput playerInput = new();

        private List<IUpdateListener> updateListeners = new();
        private List<IFixedUpdateListener> fixedUpdateListeners = new();

        private void Awake()
        {
            fade.FadeWhite();
            StartCoroutine(IWaitFadePanel(0.7f));
        }

        IEnumerator IWaitFadePanel( float time)
        {
           yield return new WaitForSeconds(time);

           Time.timeScale = 1;

            FilePrefabsPickUpItems = filePrefabsPickUpItems;
            #region Init
            if (!player)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� Player");
                yield return null;
            }

            if (!tutorialRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� TutorialRoot");
                yield return null;
            }

            if (!environmentRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� EnvironmentRoot");
                yield return null;
            }

            if (!uiGameRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� UIGameRoot");
                yield return null;
            }

            if (!filePlayer)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� FilePlayer");
                yield return null;
            }

            if (!fileShop)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� FileShop");
                yield return null;
            }
            #endregion

            apiManager.Init();

            SaveManager.Init(apiManager, filePlayer, fileShop, defaultFilePlayer, defaultFileShop);

            musicManager.Init(this);
            soundManager.Init(this);
            toastManager.Init(soundManager);


            player.Init();
            playerChangeImage.Init(player.gameObject.GetComponent<Animator>(), player);
            playerMouseMove.OnMove += playerChangeImage.Update;

            updateListeners.Add(playerMouseMove);
            playerMouseMove.Init(player.gameObject.GetComponent<NavMeshAgent>());

            updateListeners.Add(environmentRoot);
            environmentRoot.Init(player, playerMouseMove, soundManager, uiGameRoot.GetExerciseManager(), toastManager);

            updateListeners.Add(tutorialRoot);
            tutorialRoot.Init(uiGameRoot.GetDialogManager(), player);

            updateListeners.Add(uiGameRoot);
            uiGameRoot.Init(environmentRoot.tv, soundManager, toastManager);

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

        public static PickUpItem FindPickUpItemToPrefabs(string nameItem)
        {
            for (int i = 0; i < FilePrefabsPickUpItems.pickUpItems.Count; i++)
            {
                if (FilePrefabsPickUpItems.pickUpItems[i].NameItem == nameItem && nameItem.Length >= 1)
                    return FilePrefabsPickUpItems.pickUpItems[i];
            }

            return null;
        }

        #region Sound
        public void OnPlayOneShotSound(int indexSound)
        {
            soundManager.OnPlayOneShot(indexSound);
        }

        public void OnPlayOneShotSound(AudioClip audioClip)
        {
            soundManager.OnPlayOneShot(audioClip);
        }
        public void OnPlayLoopSound(int indexSound)
        {
            soundManager.OnPlayLoop(indexSound);
        }
        public void OnPlayLoopSound(AudioClip audioClip)
        {
            soundManager.OnPlayLoop(audioClip);
        }

        public void PlaySound(int indexSound)
        {
            soundManager.PlaySound(indexSound);
        }
        #endregion
        #region Music

        public void OnPlayOneShotMusic(int indexSound)
        {
            musicManager.OnPlayOneShot(indexSound);
        }

        public void OnPlayOneShotMusic(AudioClip audioClip)
        {
            musicManager.OnPlayOneShot(audioClip);
        }
        public void OnPlayLoopMusic(int indexSound)
        {
            musicManager.OnPlayLoop(indexSound);
        }
        public void OnPlayLoopMusic(AudioClip audioClip)
        {
            musicManager.OnPlayLoop(audioClip);
        }

        public void PlayMusic(int indexSound)
        {
            musicManager.PlaySound(indexSound);
        }

        #endregion
    }
}
