using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public int currentIndexScene = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeBlack()
    {
        Time.timeScale = 1;
        gameObject.SetActive(true);
        animator.SetInteger("Active", 1);
    }
    public void FadeWhite()
    {
        Time.timeScale = 1;
        gameObject.SetActive(true);
        animator.SetInteger("Active", 2);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(currentIndexScene);        
    }
}
