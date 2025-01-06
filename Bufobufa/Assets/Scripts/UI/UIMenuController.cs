using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

namespace UI
{

    public class UIMenuController : MonoBehaviour
    {
        [SerializeField] private Fade fade;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private Button startButton;
        [SerializeField] private Button continueButton;

        public void Start()
        {
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
    }
}
