using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet
{
    [RequireComponent(typeof(Animator))]
    public class PlaneTablet : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Appereance()
        {
            gameObject.SetActive(true);
            animator.SetInteger("State", 1);
        }

        public void Disappereance()
        {
            gameObject.SetActive(true);
            animator.SetInteger("State", 2);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
