using External.DI;
using External.Storage;
using Game.Dialog;
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

        private SaveManager saveManager;

        private bool isActivePause = false;

        public void Init(SaveManager saveManager)
        {
            this.saveManager = saveManager;
            fade.FadeWhite();

            shopManager.Init(saveManager);
            exerciseManager.Init(saveManager);
            dialogChat.Init(this);
            dialogManager.Init(saveManager, this);
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
            ChangeRenderMode();
            pausePanel.SetActive(true);
            isActivePause = true;
        }
        public void ContinueGame()
        {
            Time.timeScale = 1f;
            ChangeRenderMode();
            pausePanel.SetActive(false);
            isActivePause = false;
        }

        public DialogManager GetDialogManager()
        {
            return dialogManager;
        }
    }
}
