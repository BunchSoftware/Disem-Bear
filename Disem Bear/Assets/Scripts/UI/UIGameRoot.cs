using External.DI;
using External.Storage;
using Game.LDialog;
using Game.Music;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet.DialogChat;
using UI.PlaneTablet.Exercise;
using UI.PlaneTablet.Shop;
using UnityEngine;

namespace UI
{

    public class UIGameRoot : MonoBehaviour, IUpdateListener
    {
        [SerializeField] private Fade fade;
        [SerializeField] private GameObject pausePanel;
        [Header("Managers")]
        [SerializeField] private DialogManager dialogManager;
        [SerializeField] private DialogChat dialogChat;
        [SerializeField] private ShopManager shopManager;
        [SerializeField] private ExerciseManager exerciseManager;
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private SettingsController settingsController;

        private bool isActivePause = false;

        public void Init(SoundManager soundManager, ToastManager toastManager)
        {
            settingsController.Init();
            dialogManager.Init(this, soundManager);
            shopManager.Init(this, toastManager);
            exerciseManager.Init();
            dialogChat.Init(this);
            resourceManager.Init();

            Debug.Log("UIGameRoot: Успешно иницилизирован");
        }

        public void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isActivePause == false)
            {
                isActivePause = true;
                PauseGame();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isActivePause == true)
            {
                isActivePause = false;
                ContinueGame();
            }
        }

        public void ApllicationQuit()
        {
            Application.Quit();
        }

        public void LoadLevel(int buildIndex)
        {
            fade.currentIndexScene = buildIndex;
            fade.FadeBlack();
        }

        public void ChangeRenderMode()
        {
            if (gameObject.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
                gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            else if (gameObject.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay)
                gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            isActivePause = true;
        }
        public void ContinueGame()
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            isActivePause = false;
        }

        public DialogManager GetDialogManager()
        {
            return dialogManager;
        }
        public ExerciseManager GetExerciseManager()
        {
            return exerciseManager;
        }
    }
}
