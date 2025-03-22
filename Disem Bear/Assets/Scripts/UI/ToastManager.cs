using DG.Tweening;
using Game.Music;
using System;
using TMPro;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class ToastManager
    {
        [SerializeField]
        private Transform container;
        [SerializeField]
        private CanvasGroup toastPrefab;
        [SerializeField]
        private AudioClip toastSound;

        private SoundManager soundManager;

        public void Init(SoundManager soundManager)
        {
            this.soundManager = soundManager;
            Debug.Log("ToastManager: ������� ��������������");
        }

        public void ShowToast(string message)
        {
            var toast = GameObject.Instantiate(toastPrefab, container);
            toast.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
            toast.alpha = 0f;

            soundManager.OnPlayOneShot(toastSound);
            var seq = DOTween.Sequence();
            seq.Join(toast.DOFade(1f, 0.5f));
            seq.AppendInterval(2.5f);
            seq.Append(toast.DOFade(0f, 0.5f));
            seq.AppendCallback(() => GameObject.Destroy(toast.gameObject));
        }
    }
}