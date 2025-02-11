using External.DI;
using External.Storage;
using Game.Music;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

namespace UI
{
    public class UIMenuRoot : MonoBehaviour, IUpdateListener
    {
        [SerializeField] private Fade fade;
        [SerializeField] private Button startButton;
        [SerializeField] private Button continueButton;

        private SaveManager saveManager;
        private SoundManager soundManager;

        public void Init(SaveManager saveManager, SoundManager soundManager)
        {
            this.saveManager = saveManager;
            this.soundManager = soundManager;

            fade.FadeWhite();
            if (saveManager.GetJSONPlayer().resources != null && saveManager.GetJSONPlayer().resources.isPlayerRegistration)
            {
                startButton.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(true);
            }
            else
            {
                startButton.gameObject.SetActive(true);
                continueButton.gameObject.SetActive(false);
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

        public void OnUpdate(float deltaTime)
        {
           
        }
    }
}
