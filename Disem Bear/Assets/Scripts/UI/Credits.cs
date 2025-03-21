using UnityEngine;

namespace UI
{
    public class Credits : MonoBehaviour
    {
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private Animator creditsText;

        private void Update()
        {
            if (IsAnimationPlaying("CreditsAnimation") == false)
                creditsPanel.gameObject.SetActive(false);
        }

        public bool IsAnimationPlaying(string animationName)
        {
            var animatorStateInfo = creditsText.GetCurrentAnimatorStateInfo(0);
            return animatorStateInfo.IsName(animationName);
        }

        public void EndAnimation()
        {
            creditsPanel.gameObject.SetActive(false);
        }
    }
}
