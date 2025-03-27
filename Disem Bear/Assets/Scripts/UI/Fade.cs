using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Fade : MonoBehaviour
    {
        [HideInInspector] public Animator animator;
        public int currentIndexScene = 0;
        [SerializeField] private bool needNext = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (needNext)
            {
                FadeWhite();
                StartCoroutine(NextScene());
            }
        }

        IEnumerator NextScene()
        {
            yield return new WaitForSeconds(3f);
            animator.Play("FadeBlack");
        }

        public void FadeBlack()
        {
            gameObject.SetActive(true);
            animator.SetInteger("Active", 1);
        }
        public void FadeWhite()
        {
            gameObject.SetActive(true);
            animator.SetInteger("Active", 2);
        }
        public void Disable()
        {
            if (needNext)
            {
                return;
            }
            gameObject.SetActive(false);
        }

        public void LoadScene()
        {
            SceneManager.LoadScene(currentIndexScene);
        }
    }
}
