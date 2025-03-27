using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowUI : MonoBehaviour
{
    private Image Image;
    private Animator animator;
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject down;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;

    public void Init(EventsManager eventsManager)
    {
        Image = GetComponent<Image>();
        animator = GetComponent<Animator>();

        eventsManager.onEventBegin.AddListener((eventStruct) =>
        {
            up.SetActive(true);
            down.SetActive(true);
            left.SetActive(true);
            right.SetActive(true);
            animator.Play("RainBow");
        });
        eventsManager.onEventEnd.AddListener((eventStruct) =>
        {
            up.SetActive(false);
            down.SetActive(false);
            left.SetActive(false);
            right.SetActive(false);
            animator.Play("RainBowOff");
        });
    }
}
