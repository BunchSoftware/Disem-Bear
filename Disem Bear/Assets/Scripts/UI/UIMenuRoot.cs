using External.DI;
using External.Storage;
using Game.Music;
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
        private SoundManager soundManager;

        public void Init(SoundManager soundManager)
        {
            this.soundManager = soundManager;
            settingsController.Init();

            fade.FadeWhite();

            if (SaveManager.playerDatabase.JSONPlayer.resources != null && SaveManager.playerDatabase.JSONPlayer.resources.isPlayerRegistration)
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
            SaveManager.ResetFilePlayer();
            SaveManager.ResetFileShop();
        }

        public void OnUpdate(float deltaTime)
        {

        }
    }
}
