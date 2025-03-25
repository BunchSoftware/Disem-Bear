using Game.Music;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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

        public UnityEvent<string> OnSendInputFieldText;
        private bool isDialogRun = false;

        private DialogManager dialogManager;
        private SoundManager soundManager;
        private Font standartFont;
        private bool skipDialog = false;
        private bool oneTap = false;
        private Coroutine typeLineCoroutine;

        public void Init(DialogManager dialogManager, SoundManager soundManager)
        {
            this.soundManager = soundManager;
            this.dialogManager = dialogManager;
            standartFont = textDialog.font;

            dialogInputField.OnSendInputFieldText.AddListener((text) =>
            {
                OnSendInputFieldText?.Invoke(text);
            });

            animator = GetComponentInChildren<Animator>();

            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener((UnityAction)(() =>
            {
                if (isDialogRun)
                {
                    if (skipDialog && !oneTap)
                    {
                        oneTap = true;
                        dialogManager.SkipReplicaWithFinish();
                    }
                    else if (oneTap)
                    {
                        oneTap = false;
                        dialogManager.SkipReplica();
                    }
                }
                else
                {
                    if(skipDialog)
                        dialogManager.SkipReplica();
                }
            }));

            dialogInputField.Init(dialogManager);
            Debug.Log("DialogueWindow: Успешно иницилизирован");
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

        private IEnumerator TypeLineIE(Dialog dialog)
        {
            textDialog.text = "";
            UpdateData(dialog);
            dialogInputField.UpdateData(dialog);

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
            UpdateData(dialog);
            dialogInputField.UpdateData(dialog);
            textDialog.text = dialog.textDialog;
            isDialogRun = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        }

        private void UpdateData(Dialog dialog)
        {
            textDialog.font = dialog.fontText != null ? dialog.fontText : standartFont;

            skipDialog = dialog.skipDialogButton;

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
