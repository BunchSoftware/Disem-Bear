using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameControl : MonoBehaviour
{
    [SerializeField] private Fade fade;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GameObject pausePanel;

    private bool isActivePause = false;

    public void Start()
    {
        fade.FadeWhite();
    }

    private void Update()
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
        else if(gameObject.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay)
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
}
