using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMessage : MonoBehaviour
{
    public Text textMessage;
    public Image iconMessage;
    [HideInInspector] public Animator animator;

    public void Init()
    {
        animator = GetComponent<Animator>();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
