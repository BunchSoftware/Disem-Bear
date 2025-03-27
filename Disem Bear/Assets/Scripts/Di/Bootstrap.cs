using External.API;
using External.DI;
using External.Storage;
using Game.Music;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("Save System")]
    [SerializeField] protected PlayerDatabase defaultFilePlayer;
    [SerializeField] protected ShopDatabase defaultFileShop;
    [SerializeField] protected UGCDatabase defaultFileUGC;
    [SerializeField] protected PlayerDatabase filePlayer;
    [SerializeField] protected ShopDatabase fileShop;
    [SerializeField] protected UGCDatabase fileUGC;
    [SerializeField] protected APIManager apiManager;
    [Header("Sound")]
    [SerializeField] protected SoundManager soundManager;
    [SerializeField] protected SoundManager musicManager;

    protected List<IUpdateListener> updateListeners = new();
    protected List<IFixedUpdateListener> fixedUpdateListeners = new();

    protected const float TimeScale = 1f;

    protected void Init()
    {

        if (!filePlayer)
        {
            Debug.LogError("CriticError-Bootstrap: Не указано значение переменной FilePlayer");
            return;
        }

        if (!fileShop)
        {
            Debug.LogError("CriticError-Bootstrap: Не указано значение переменной FileShop");
            return;
        }

        apiManager.Init();

        SaveManager.Init(apiManager, filePlayer, fileShop, fileUGC, defaultFilePlayer, defaultFileShop, defaultFileUGC);

        musicManager.Init(this);
        soundManager.Init(this);
    }

    protected void Update()
    {
        var deltaTime = Time.deltaTime;
        for (int i = 0, count = updateListeners.Count; i < count; i++)
        {
            var listener = updateListeners[i];
            listener.OnUpdate(deltaTime);
        }
    }

    protected void FixedUpdate()
    {
        var fixedDeltaTime = Time.fixedDeltaTime;
        for (int i = 0, count = fixedUpdateListeners.Count; i < count; i++)
        {
            var listener = fixedUpdateListeners[i];
            listener.OnFixedUpdate(fixedDeltaTime);
        }
    }

    public void AddUpdateListener(IUpdateListener updateListener)
    {
        updateListeners.Add(updateListener);
    }

    public void RemoveUpdateListener(IUpdateListener updateListener)
    {
        updateListeners.Remove(updateListener);
    }

    public void AddFixedUpdateListener(IFixedUpdateListener fixedUpdateListener)
    {
        fixedUpdateListeners.Add(fixedUpdateListener);
    }

    public void RemoveFixedUpdateListener(IFixedUpdateListener fixedUpdateListener)
    {
        fixedUpdateListeners.Remove(fixedUpdateListener);
    }

    #region Sound
    public AudioSource OnPlayOneShotRandomSound(List<AudioClip> sounds)
    {
        if (sounds.Count > 0)
        {
            AudioSource audio = OnPlayOneShotSound(sounds[DateTime.Now.Second % sounds.Count]);
            return audio;
        }
        else
        {
            Debug.Log("Не указан звук");
            return null;
        }
    }

    public AudioSource OnPlayOneShotSound(int indexSound)
    {
        AudioSource audio = soundManager.OnPlayOneShot(indexSound);
        return audio;
    }

    public AudioSource OnPlayOneShotSound(AudioClip audioClip)
    {
        AudioSource audio = null;
        if (audioClip != null)
            audio = soundManager.OnPlayOneShot(audioClip);
        else
            Debug.Log("Не указан звук");
        return audio;
    }
    public AudioSource OnPlayLoopSound(int indexSound)
    {
        AudioSource audio = soundManager.OnPlayLoop(indexSound);
        return audio;
    }
    public AudioSource OnPlayLoopSound(AudioClip audioClip)
    {
        AudioSource audio = null;
        if (audioClip != null)
            soundManager.OnPlayLoop(audioClip);
        else
            Debug.Log("Не указан звук");
        return audio;
    }
    public void OnEndPlayOneSHotSound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            soundManager.OnEndPlayOneShot(audioSource);
        }
        else
        {
            Debug.Log("Не указан проигрыватель");
        }
    }

    public void PlaySound(int indexSound)
    {
        soundManager.PlaySound(indexSound);
    }
    #endregion
    #region Music

    public void OnPlayOneShotMusic(int indexSound)
    {
        musicManager.OnPlayOneShot(indexSound);
    }

    public void OnPlayOneShotMusic(AudioClip audioClip)
    {
        musicManager.OnPlayOneShot(audioClip);
    }
    public void OnPlayLoopMusic(int indexSound)
    {
        musicManager.OnPlayLoop(indexSound);
    }
    public void OnPlayLoopMusic(AudioClip audioClip)
    {
        musicManager.OnPlayLoop(audioClip);
    }

    public void PlayMusic(int indexSound)
    {
        musicManager.PlaySound(indexSound);
    }

    #endregion
}
