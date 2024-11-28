using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class UIMenuController : MonoBehaviour
{
    [SerializeField] private Fade fade;

    public void Start()
    {
        fade.FadeWhite();
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
