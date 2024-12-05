using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


[System.Serializable]
public class Dialog
{
    [Header("Settings Dialog")]
    public DropEnum enterDrop = DropEnum.DropRight;
    public DropEnum exitDrop = DropEnum.DropLeft;
    public float speedText = 0.05f;
    public bool skipDialog = false;
    public string conditionSkipDialog;
    public bool stopTheEndDialog = false;
    public float waitSecond = 0;
    [Header("Settings TextDialog")]
    public Color colorText = Color.black;
    public FontStyle fontStyleText = FontStyle.Normal;
    public Font fontText;
    public int fontSizeText = 40;
    public Sprite avatar;
    [TextArea(10, 100)]
    public string textDialog = "";
    public List<DialogChoice> dialogChoices;
    [Header("Setting Input Field")]
    public bool isActiveInputField = false;

    public Color colorTextInputField = Color.black;
    public FontStyle fontStyleTextInputField = FontStyle.Normal;
    public Font fontTextInputField;
    public int fontSizeTextInputField = 40;

    public Color colorPlaceHolderText = Color.black;
    public FontStyle fontStylePlaceHolderText = FontStyle.Normal;
    public Font fontPlaceHolderText;
    public int fontSizePlaceHolderText = 40;
    public string textPlaceHolderText = "";
}
