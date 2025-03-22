using Game.LDialog;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PlaneTablet.DialogChat
{
    public class DialogChoiceButton : MonoBehaviour
    {
        [SerializeField] private Text textButton;
        [SerializeField] private Button button;

        public void Init(Action<int> ActionChoiceButton, DialogChoice dialogChoice)
        {
            UpdateData(dialogChoice);
            button.onClick.AddListener(() =>
            {
                ActionChoiceButton?.Invoke(dialogChoice.indexDialogPoint);
            });
        }

        public void UpdateData(DialogChoice dialogChoice)
        {
            textButton.fontStyle = dialogChoice.fontStyleText;
            textButton.fontSize = dialogChoice.fontSizeText;
            textButton.color = dialogChoice.colorText;
            textButton.text = dialogChoice.textButtonChoice;
            button.GetComponent<Image>().color = dialogChoice.colorButton;

        }
    }
}
