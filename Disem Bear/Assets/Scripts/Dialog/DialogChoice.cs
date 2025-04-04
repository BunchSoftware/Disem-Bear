using System;
using UnityEngine;

namespace Game.LDialog
{
    [Serializable]
    public class DialogChoice
    {
        public Color colorButton = Color.white;
        [Header("Text")]
        public Color colorText = Color.black;
        public FontStyle fontStyleText = FontStyle.Normal;
        public Font fontText;
        public int fontSizeText = 40;
        public string textButtonChoice;
        public int indexDialogPoint;
    }
}
