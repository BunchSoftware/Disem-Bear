using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace UI
{

    public class IntroController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private Fade fadePanel;
        [SerializeField] private List<VideoClip> videoClips;
        [SerializeField] private int indexScene;
        private int currentVideoClips;

        private void Start()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            fadePanel.FadeWhite();
            StartCoroutine(PlayVideo());
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                fadePanel.currentIndexScene = indexScene;
                fadePanel.FadeBlack();
            }
        }
        private void OnDestroy()
        {
            videoPlayer.loopPointReached -= EndReached;
        }
        private IEnumerator PlayVideo()
        {
            for (int i = 0; i < videoClips.Count; i++)
            {
                currentVideoClips = i;
                if (i == videoClips.Count - 1)
                    videoPlayer.loopPointReached += EndReached;
                videoPlayer.clip = videoClips[currentVideoClips];
                videoPlayer.Play();
                yield return new WaitForSeconds((float)videoClips[currentVideoClips].length - videoPlayer.playbackSpeed);
            }
        }
        private void EndReached(VideoPlayer vp)
        {
            fadePanel.currentIndexScene = indexScene;
            fadePanel.FadeBlack();
        }
    }
}
