using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private List<VideoClip> videoClips;
    [SerializeField] private string nameScene;
    private int currentVideoClips;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(PlayVideo());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SceneManager.LoadScene(nameScene);
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
        SceneManager.LoadScene(nameScene);
    }
}
