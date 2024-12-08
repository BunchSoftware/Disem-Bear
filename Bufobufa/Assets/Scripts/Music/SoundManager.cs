using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

// Скрипт для управления музыкой, эффектами и звуками через AudioMixer
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private AudioSource audio;
    [SerializeField] private AudioMixerGroup mixer;
    // Ключ, название которого совпадает с названием AudioMixer и названием переменной в реестре
    [SerializeField] private string nameKey;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private SoundClip playAwake;
    [Header("Настройки")]
    [SerializeField] private List<SoundClip> soundClips;
    [Range(0f, -100f)]
    [SerializeField] private float MinDB = -40;
    [Range(-100f, 20f)]
    [SerializeField] private float MaxDB = 10;

    public void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.mute = false;
        audio.loop = false;
        audio.playOnAwake = false;

        // Проверка на наличие слайдера регулировки звука
        if (mixer == null)
            Debug.LogError("В настройках звука установите Audio Mixer");
        if (soundSlider != null)
        {
            soundSlider.onValueChanged.RemoveAllListeners();
            soundSlider.onValueChanged.AddListener((value) =>
            {
                ChangeVolume();
            });
            soundSlider.value = PlayerPrefs.GetFloat(nameKey, 1f);

            if (Mathf.Lerp(MinDB, MaxDB, soundSlider.value) == MinDB)
                mixer.audioMixer.SetFloat(nameKey, -80f);
            else
                mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, soundSlider.value));
        }
        else
        {
            if (PlayerPrefs.HasKey(nameKey) )
                mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, PlayerPrefs.GetFloat(nameKey, 1f)));
            else
                mixer.audioMixer.SetFloat(nameKey, 20);
        }


        if (playAwake != null)
        {
            audio.Stop();
            audio.clip = playAwake.audioClip;
            audio.loop = playAwake.isLoop;
            audio.Play();
        }
    }

    // Эффект затухания громокости звука
    private IEnumerator DecayIEnumarator(float time)
    {
        float volume;
        mixer.audioMixer.GetFloat(nameKey, out volume);

        while (volume > MinDB)
        {
            volume -= -MinDB * Time.deltaTime / time;
            mixer.audioMixer.SetFloat(nameKey, volume);
            yield return null;
        }
    }
    // Эффект повышения громкости звука
    private IEnumerator ResurrectionIEnumarator(float time)
    {
        float tempVolume = 0;

        if (PlayerPrefs.HasKey(nameKey))
            mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, PlayerPrefs.GetFloat(nameKey, 1f)));
        else
            mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, 1));

        mixer.audioMixer.GetFloat(nameKey, out tempVolume);
        mixer.audioMixer.SetFloat(nameKey, MinDB);
        float volume = MinDB;

        while (tempVolume > volume)
        {
            volume += -MinDB * Time.deltaTime / time;
            mixer.audioMixer.SetFloat(nameKey, volume);
            yield return null;
        }
    }

    // Чтобы запускать звук один раз по индексу в списке звуков
    public void OnPlayOneShot(int indexSound)
    {
        audio.loop = false;
        if (indexSound >= 0 && indexSound <= soundClips.Count)
        {
            audio.PlayOneShot(soundClips[indexSound].audioClip);
        }
        else
            Debug.LogError("Выход за рамки массива звуков");
    }

    // Чтобы запускать звук один раз по исходному файлу звука
    public void OnPlayOneShot(AudioClip audioClip)
    {
        audio.loop = false;
        if (!audio.isPlaying)
            audio.PlayOneShot(audioClip);
    }
    // Запуск звука с режимом бесконечного повторения
    public void OnPlayLoop(int indexSound)
    {
        if (indexSound >= 0 && indexSound <= soundClips.Count)
        {
            audio.loop = true;
            audio.clip = soundClips[indexSound].audioClip;
            audio.Play();
        }
        else
            Debug.LogError("Выход за рамки массива звуков");
    }
    public void OnPlayLoop(AudioClip audioClip)
    {
        audio.loop = true;
        audio.clip = audioClip;
        audio.Play();
    }

    public void PlaySound(int indexSound)
    {
        if (indexSound >= 0 && indexSound <= soundClips.Count)
        {
            audio.clip = soundClips[indexSound].audioClip;
            audio.loop = soundClips[indexSound].isLoop;
            audio.Play();
        }
        else
            Debug.LogError("Выход за рамки массива звуков");
    }
    // Остановить воспроизведение звуков
    public void Stop()
    {
        if (audio.isPlaying)
            audio.Stop();
    }

    public void Play()
    {
        if (audio.isPlaying == false)
            audio.Play();
    }

    // Чтобы узнавать ValueSlider
    public float InfoSlider()
    {
        return soundSlider.value;
    }
    // Для Slider чтобы изменять громкость
    private void ChangeVolume()
    {
        if (Mathf.Lerp(MinDB, MaxDB, soundSlider.value) == MinDB)
        {
            mixer.audioMixer.SetFloat(nameKey, -80);
            PlayerPrefs.SetFloat(nameKey, MinDB);
        }
        else
        {
            mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, soundSlider.value));
            PlayerPrefs.SetFloat(nameKey, soundSlider.value);
        }
    }
    // Включения звука
    public void OnSound()
    {
        audio.mute = true;
    }
    // Выключение звука
    public void OffSound()
    {
        audio.mute = false;
    }

    public void SoundDecay(float time)
    {
        StartCoroutine(DecayIEnumarator(time));
    }
    public void SoundResurrection(float time)
    {
        StartCoroutine(ResurrectionIEnumarator(time));
    }
}
