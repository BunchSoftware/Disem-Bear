using UnityEngine;
using UnityEngine.EventSystems;

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
            Camera.main.GetComponent<PhysicsRaycaster>().enabled = false;
            gameObject.SetActive(true);
            animator.SetInteger("State", 1);
        }

        public void Disappereance()
        {
            Camera.main.GetComponent<PhysicsRaycaster>().enabled = true;
            animator.SetInteger("State", 2);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
