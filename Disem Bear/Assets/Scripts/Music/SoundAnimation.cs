using External.DI;
using Game.Music;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAnimation : MonoBehaviour
{
    private GameBootstrap gameBootstrap;

    public void Awake()
    {
        gameBootstrap = FindFirstObjectByType<GameBootstrap>();
    }

    #region Sound
    public void OnPlayOneShotSoundIndex(int indexSound)
    {
        gameBootstrap.OnPlayOneShotSound(indexSound);
    }

    public void OnPlayOneShotSound(AudioClip audioClip)
    {
        gameBootstrap.OnPlayOneShotSound(audioClip);
    }
    public void OnPlayLoopSoundIndex(int indexSound)
    {
        gameBootstrap.OnPlayLoopSound(indexSound);
    }
    public void OnPlayLoopSound(AudioClip audioClip)
    {
        gameBootstrap.OnPlayLoopSound(audioClip);
    }

    public void PlaySoundIndex(int indexSound)
    {
        gameBootstrap.PlaySound(indexSound);
    }
    #endregion
    #region Music

    public void OnPlayOneShotMusicIndex(int indexSound)
    {
        gameBootstrap.OnPlayOneShotMusic(indexSound);
    }

    public void OnPlayOneShotMusic(AudioClip audioClip)
    {
        gameBootstrap.OnPlayOneShotMusic(audioClip);
    }
    public void OnPlayLoopMusicIndex(int indexSound)
    {
        gameBootstrap.OnPlayOneShotMusic(indexSound);
    }
    public void OnPlayLoopMusic(AudioClip audioClip)
    {
        gameBootstrap.OnPlayOneShotMusic(audioClip);
    }

    public void PlayMusicIndex(int indexSound)
    {
        gameBootstrap.PlayMusic(indexSound);
    }

    #endregion
}
