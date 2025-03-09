using External.DI;
using External.Storage;
using Game.Music;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuRoot : MonoBehaviour, IUpdateListener
    {
        [SerializeField] private Fade fade;
        [SerializeField] private Button startButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private SettingsController settingsController;

        private SaveManager saveManager;
        private SoundManager soundManager;

        public void Init(SaveManager saveManager, SoundManager soundManager)
        {
            this.saveManager = saveManager;
            this.soundManager = soundManager;
            settingsController.Init();

            fade.FadeWhite();
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
            if (saveManager.filePlayer.JSONPlayer.resources != null && saveManager.filePlayer.JSONPlayer.resources.isPlayerRegistration)
            {
                startButton.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(true);
                newGameButton.gameObject.SetActive(true);
            }
            else
            {
                startButton.gameObject.SetActive(true);
                continueButton.gameObject.SetActive(false);
                newGameButton.gameObject.SetActive(false);
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

        public void OnPlayOneShot(AudioClip audioClip)
        {
            soundManager.OnPlayOneShot(audioClip);
        }

        public void NewGame()
        {
            PlayerPrefs.SetInt("isDefault", 1);
            saveManager.ResetFilePlayer();
            saveManager.ResetFileShop();
        }

        public void OnUpdate(float deltaTime)
        {
           
        }
    }
}
