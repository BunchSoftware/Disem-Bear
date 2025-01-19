using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PlaneTablet.DialogChat
{
    public class DialogMessage : MonoBehaviour
    {
        public Text textMessage;
        public Image iconMessage;
        [HideInInspector] public Animator animator;

        public void Init()
        {
            animator = GetComponent<Animator>();
            gameObject.SetActive(true);
        }
    }
}
