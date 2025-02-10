using Game.LDialog;
using System;
using System.Collections;
using System.Collections.Generic;
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
            textButton.fontStyle = dialogChoice.fontStyleText;
            textButton.fontSize = dialogChoice.fontSizeText;
            textButton.color = dialogChoice.colorText;
            textButton.text = dialogChoice.textButtonChoice;
            button.GetComponent<Image>().color = dialogChoice.colorButton;

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() =>
            {
                ActionChoiceButton?.Invoke(dialogChoice.indexDialogPoint);
            });
            //button.GetComponent<Animator>().SetInteger("State", 1);
        }
    }
}
