using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

// Скрипт для управления музыкой, эффектами и звуками через AudioMixer
public class MusicManager : MonoBehaviour
{
    private AudioSource Audio;
    public static MusicManager instance;
    [SerializeField] private AudioMixerGroup Mixer;
    // Ключ, название которого совпадает с названием AudioMixer и названием переменной в реестре
    [SerializeField] private string nameKey;
    [SerializeField] private Slider SoundSlider;
    [SerializeField] private bool OnPlayAwake = false;
    [Header("Настройки")]
    [SerializeField] private AudioClip[] audioClip;
    [Range(0f, -100f)]
    [SerializeField] private float MinDB;
    [Range(-100f, 20f)]
    [SerializeField] private float MaxDB;

    public void Awake()
    {
        Audio = GetComponent<AudioSource>();
        // Проверка на наличие слайдера регулировки звука
        if (SoundSlider != null)
        {
            SoundSlider.value = PlayerPrefs.GetFloat(nameKey, 1f);
            if (Mathf.Lerp(MinDB, MaxDB, SoundSlider.value) == MinDB)
            {
                Mixer.audioMixer.SetFloat(nameKey, -80f);
            }
            else
            {
                Mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, SoundSlider.value));
            }

            if (OnPlayAwake)
                OnPlayOneShot(0);
        }
        else
        {
            if (PlayerPrefs.HasKey(nameKey) )
                Mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, PlayerPrefs.GetFloat(nameKey, 1f)));
            else
                Mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, 1));

            if (OnPlayAwake)
                OnPlayLoop(0);
        }
    }

    // Эффект затухания громокости звука
    private IEnumerator DecayIEnumarator(float time)
    {
        float volume;
        Mixer.audioMixer.GetFloat(nameKey, out volume);

        while (volume > MinDB)
        {
            volume -= -MinDB * Time.deltaTime / time;
            Mixer.audioMixer.SetFloat(nameKey, volume);
            yield return null;
        }
    }
    // Эффект повышения громкости звука
    private IEnumerator ResurrectionIEnumarator(float time)
    {
        float tempVolume = 0;

        if (PlayerPrefs.HasKey(nameKey))
            Mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, PlayerPrefs.GetFloat(nameKey, 1f)));
        else
            Mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, 1));

        Mixer.audioMixer.GetFloat(nameKey, out tempVolume);
        Mixer.audioMixer.SetFloat(nameKey, MinDB);
        float volume = MinDB;

        while (tempVolume > volume)
        {
            volume += -MinDB * Time.deltaTime / time;
            Mixer.audioMixer.SetFloat(nameKey, volume);
            yield return null;
        }
    }

    // Чтобы запускать звук один раз по индексу в списке звуков
    public void OnPlayOneShot(int number)
    {
        if (!Audio.isPlaying & audioClip.Length != 0 & number <= audioClip.Length - 1)
        {
            Audio.PlayOneShot(audioClip[number]);
        }
    }

    // Чтобы запускать звук один раз по исходному файлу звука
    public void OnPlayOneShot(AudioClip audioClip)
    {
        if (!Audio.isPlaying)
        {
            Audio.PlayOneShot(audioClip);
        }
    }
    // Запуск звука с режимом бесконечного повтореня
    public void OnPlayLoop(int number)
    {
        if (!Audio.isPlaying & audioClip.Length != 0 & number <= audioClip.Length - 1)
        {
            Audio.loop = true;
            Audio.clip = audioClip[number];
            Audio.Play();
        }
    }
    // Чтобы закончить прошлый звук и воспроизвести новый звук один раз по исходному файлу звука
    public void OnPlayOneShotAndEndLast(AudioClip audioClip)
    {
        Stop();
        Audio.PlayOneShot(audioClip);
    }
    public void OnPlayOneShotAndEndLast(int number)
    {
        Stop();
        if (audioClip.Length != 0 & number <= audioClip.Length - 1)
        {
            Audio.PlayOneShot(audioClip[number]);
        }
    }

    // Остановить воспроизведение звуков
    public void Stop()
    {
        if (Audio.isPlaying)
            Audio.Stop();
    }

    // Чтобы узнавать ValueSlider
    public float InfoSlider()
    {
        return SoundSlider.value;
    }
    // Для Slider чтобы изменять громкость
    public void ChangeVolume()
    {
        if (Mathf.Lerp(MinDB, MaxDB, SoundSlider.value) == MinDB)
        {
            Mixer.audioMixer.SetFloat(nameKey, -80);
            PlayerPrefs.SetFloat(nameKey, MinDB);
        }
        else
        {
            Mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, SoundSlider.value));
            PlayerPrefs.SetFloat(nameKey, SoundSlider.value);
        }
    }
    // Включения звука
    public void OnSound()
    {
        Audio.mute = true;
    }
    // Выключение звука
    public void OffSound()
    {
        Audio.mute = false;
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
