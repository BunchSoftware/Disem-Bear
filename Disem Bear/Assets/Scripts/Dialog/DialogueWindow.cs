using Game.Music;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.LDialog
{

    public class DialogueWindow : MonoBehaviour
    {
        [SerializeField] private Text textDialog;
        [SerializeField] private Image iconDialog;
        [SerializeField] private Button skipButton;
        [SerializeField] private DialogInputField dialogInputField;
        [SerializeField] private Image imageClickButtonForSkip;
        [HideInInspector] public Animator animator;
        private bool isDialogRun = false;

        private SoundManager soundManager;
        private Font standartFont;
        private bool skipDialog = false;
        private bool oneTap = false;
        private Coroutine typeLineCoroutine;

        public void Init(DialogManager dialogManager, SoundManager soundManager)
        {
            this.soundManager = soundManager;

            animator = GetComponentInChildren<Animator>();
            standartFont = textDialog.font;
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(() =>
            {
                if (dialogManager.IsDialogRun())
                {
                    if (skipDialog && !oneTap)
                    {
                        oneTap = true;
                        dialogManager.SkipDialogWithFinish();
                    }
                    else if (oneTap)
                    {
                        oneTap = false;
                        dialogManager.RunConditionSkip("");
                    }
                }
                else
                {
                    dialogManager.RunConditionSkip("");
                }
            });

            dialogInputField.Init(dialogManager);
        }

        public void StartTypeLine(Dialog dialog)
        {
            if (typeLineCoroutine != null)
                StopCoroutine(typeLineCoroutine);
            typeLineCoroutine = StartCoroutine(TypeLineIE(dialog));
        }

        public void StopTypeLine()
        {
            if (typeLineCoroutine != null)
                StopCoroutine(typeLineCoroutine);
            isDialogRun = false;
        }

        IEnumerator TypeLineIE(Dialog dialog)
        {
            textDialog.text = "";
            SetParametres(dialog);
            dialogInputField.SetParametres(dialog);

            isDialogRun = true;

            for (int j = 0; j < dialog.textDialog.ToCharArray().Length; j++)
            {
                yield return new WaitForSeconds(dialog.speedText);
                if (dialog.soundText != null)
                    soundManager.OnPlayOneShot(dialog.soundText);

                textDialog.text += dialog.textDialog[j];
            }

            isDialogRun = false;
        }

        public void ShowFullDialog(Dialog dialog)
        {
            if (typeLineCoroutine != null)
                StopCoroutine(typeLineCoroutine);
            SetParametres(dialog);
            dialogInputField.SetParametres(dialog);
            textDialog.text = dialog.textDialog;
            isDialogRun = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        }

        private void SetParametres(Dialog dialog)
        {
            if (dialog.fontText != null)
                textDialog.font = dialog.fontText;
            else
                textDialog.font = standartFont;

            skipDialog = dialog.skipDialog;

            textDialog.fontStyle = dialog.fontStyleText;
            textDialog.fontSize = dialog.fontSizeText;
            textDialog.color = dialog.colorText;
            iconDialog.sprite = dialog.avatar;
            iconDialog.preserveAspect = true;

            if (dialog.imageClickButtonForSkip != null)
            {
                imageClickButtonForSkip.sprite = dialog.imageClickButtonForSkip;
                imageClickButtonForSkip.preserveAspect = true;
                imageClickButtonForSkip.gameObject.SetActive(true);
            }
            else
                imageClickButtonForSkip.gameObject.SetActive(false);

            dialogInputField.gameObject.SetActive(dialog.isActiveInputField);
        }


        public bool IsDialogRun()
        {
            return isDialogRun;
        }
    }
}
