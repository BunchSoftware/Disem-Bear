using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using External.DI;

namespace Game.Music
{
    [Serializable]
    public class SoundManager
    {
        [SerializeField] private AudioSource prefabAudioSource;
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

        private List<AudioSource> audios = new List<AudioSource>();

        private MonoBehaviour context;

        public void Init(MonoBehaviour context)
        {
            this.context = context;

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
                if (PlayerPrefs.HasKey(nameKey))
                    mixer.audioMixer.SetFloat(nameKey, Mathf.Lerp(MinDB, MaxDB, PlayerPrefs.GetFloat(nameKey, 1f)));
                else
                    mixer.audioMixer.SetFloat(nameKey, 20);
            }


            if (playAwake.audioClip != null)
            {
                AudioSource audio = GameObject.Instantiate(prefabAudioSource);
                audio.clip = playAwake.audioClip;
                audio.loop = playAwake.isLoop;
                audio.Play();

                audios.Add(audio);

                if(!audio.loop)
                    context.StartCoroutine(EndAudioClip(audio));
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

        private IEnumerator EndAudioClip(AudioSource audioSource)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            audios.Remove(audioSource);
            GameObject.Destroy(audioSource.gameObject);
        }

        private IEnumerator EndAudioClip(AudioSource audioSource, AudioClip audioClip)
        {
            yield return new WaitForSeconds(audioClip.length);
            audios.Remove(audioSource);
            GameObject.Destroy(audioSource.gameObject);
        }

        // Чтобы запускать звук один раз по индексу в списке звуков
        public void OnPlayOneShot(int indexSound)
        {
            if (indexSound >= 0 && indexSound <= soundClips.Count)
            {
                AudioSource audio = GameObject.Instantiate(prefabAudioSource);
                audio.loop = false;

                audios.Add(audio);
                audio.PlayOneShot(soundClips[indexSound].audioClip);

                context.StartCoroutine(EndAudioClip(audio, soundClips[indexSound].audioClip));
            }
            else
                Debug.LogError("Выход за рамки массива звуков");
        }

        // Чтобы запускать звук один раз по исходному файлу звука
        public void OnPlayOneShot(AudioClip audioClip)
        {
            AudioSource audio = GameObject.Instantiate(prefabAudioSource);
            audio.loop = false;

            audios.Add(audio);
            audio.PlayOneShot(audioClip);

            context.StartCoroutine(EndAudioClip(audio, audioClip));
        }
        // Запуск звука с режимом бесконечного повторения
        public void OnPlayLoop(int indexSound)
        {
            if (indexSound >= 0 && indexSound <= soundClips.Count)
            {
                AudioSource audio = GameObject.Instantiate(prefabAudioSource);
                audio.loop = true;
                audio.clip = soundClips[indexSound].audioClip;
                audio.Play();

                audios.Add(audio);
            }
            else
                Debug.LogError("Выход за рамки массива звуков");
        }
        public void OnPlayLoop(AudioClip audioClip)
        {
            AudioSource audio = GameObject.Instantiate(prefabAudioSource);
            audio.loop = true;
            audio.clip = audioClip;
            audio.Play();

            audios.Add(audio);
        }

        public void PlaySound(int indexSound)
        {
            if (indexSound >= 0 && indexSound <= soundClips.Count)
            {
                AudioSource audio = GameObject.Instantiate(prefabAudioSource);
                audio.loop = soundClips[indexSound].isLoop;
                audio.clip = soundClips[indexSound].audioClip;
                audio.Play();

                audios.Add(audio);

                if(!audio.loop)
                    context.StartCoroutine(EndAudioClip(audio));
            }
            else
                Debug.LogError("Выход за рамки массива звуков");
        }
        // Остановить воспроизведение звуков
        public void Stop()
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (audios[i].isPlaying)
                    audios[i].Stop();
            }

            context.StopAllCoroutines();
        }

        public void Play()
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (audios[i].isPlaying == false)
                    audios[i].Play();
            }
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
            for (int i = 0; i < audios.Count; i++)
            {
                audios[i].mute = true;
            }
        }
        // Выключение звука
        public void OffSound()
        {
            for (int i = 0; i < audios.Count; i++)
            {
                audios[i].mute = false;
            }
        }

        public void SoundDecay(float time)
        {
            context.StartCoroutine(DecayIEnumarator(time));
        }
        public void SoundResurrection(float time)
        {
            context.StartCoroutine(ResurrectionIEnumarator(time));
        }
    }
}
