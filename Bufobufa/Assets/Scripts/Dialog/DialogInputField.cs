using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dialog
{
    public class DialogInputField : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Text inputText;
        [SerializeField] private Text placeHolderText;
        [SerializeField] private Button sendButton;

        private Font standartInputFont;
        private Font standartPlaceholderFont;

        public void Init(DialogManager dialogManager)
        {
            standartInputFont = inputText.font;
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(() =>
            {
                dialogManager.SendInputText(inputField.text);
            });
        }

        public void SetParametres(Dialog dialog)
        {
            if (dialog.fontText != null)
                inputText.font = dialog.fontText;
            else
                inputText.font = standartInputFont;

            if (dialog.fontText != null)
                placeHolderText.font = dialog.fontText;
            else
                placeHolderText.font = standartPlaceholderFont;

            inputText.color = dialog.colorTextInputField;
            inputText.fontStyle = dialog.fontStyleTextInputField;
            inputText.fontSize = dialog.fontSizeTextInputField;

            placeHolderText.color = dialog.colorPlaceHolderText;
            placeHolderText.fontStyle = dialog.fontStylePlaceHolderText;
            placeHolderText.fontSize = dialog.fontSizePlaceHolderText;
            placeHolderText.text = dialog.textPlaceHolderText;
        }
    }
}

