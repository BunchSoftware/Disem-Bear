using Game.LDialog;
using System.Collections;
using UnityEngine;

namespace UI.PlaneTablet.DialogChat
{

    public enum DialogSide
    {
        Right = 0,
        Left = 1,
    }

    public class DialogMessageGroup : MonoBehaviour
    {
        [SerializeField] private DialogMessage messageRight;
        [SerializeField] private DialogMessage messageLeft;

        private DialogSide currentDialogSide;
        [HideInInspector] public DialogMessage currentMessage;

        public void Init(DialogChat dialogChat, DialogSide dialogSide)
        {
            switch (dialogSide)
            {
                case DialogSide.Right:
                    {
                        currentMessage = messageRight;
                    }
                    break;
                case DialogSide.Left:
                    {
                        currentMessage = messageLeft;
                    }
                    break;
            }
            messageLeft.gameObject.SetActive(false);
            messageRight.gameObject.SetActive(false);
            currentMessage.Init();
        }

        public void StartTypeLine(Dialog dialog)
        {
            StopAllCoroutines();
            StartCoroutine(TypeLineIE(dialog));
        }

        public void StopTypeLine()
        {
            StopAllCoroutines();
        }

        private IEnumerator TypeLineIE(Dialog dialog)
        {
            currentMessage.textMessage.text = "";
            UpdateData(dialog);
            for (int j = 0; j < dialog.textDialog.ToCharArray().Length; j++)
            {
                currentMessage.textMessage.text += dialog.textDialog[j];
                yield return new WaitForSeconds(dialog.speedText);
            }
        }

        public void DialogFinish(Dialog dialog)
        {
            UpdateData(dialog);
            currentMessage.textMessage.text = dialog.textDialog;
        }

        private void UpdateData(Dialog dialog)
        {
            currentMessage.textMessage.fontStyle = dialog.fontStyleText;
            currentMessage.textMessage.fontSize = dialog.fontSizeText;
            currentMessage.textMessage.color = dialog.colorText;
            currentMessage.iconMessage.sprite = dialog.avatar;
        }
    }
}
