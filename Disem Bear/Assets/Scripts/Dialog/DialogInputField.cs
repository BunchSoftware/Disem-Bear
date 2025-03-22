using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Game.LDialog
{
    public class DialogInputField : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Text inputText;
        [SerializeField] private Text placeHolderText;
        [SerializeField] private Button sendButton;

        public UnityEvent<string> OnSendInputFieldText;
        private Font standartInputFont;
        private Font standartPlaceholderFont;

        private const int MinLenghtCountInputField = 1;

        public void Init(DialogManager dialogManager)
        {
            standartInputFont = inputText.font;
            standartPlaceholderFont = placeHolderText.font;

            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(() =>
            {
                if (inputField.text.Length > MinLenghtCountInputField)
                    OnSendInputFieldText?.Invoke(inputField.text);
            });
            Debug.Log("DialogInputField: Успешно иницилизирован");
        }

        public void UpdateData(Dialog dialog)
        {
            inputText.font = dialog.fontText != null ? dialog.fontText : standartInputFont;

            placeHolderText.font = dialog.fontText != null ? dialog.fontText : standartPlaceholderFont;

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

