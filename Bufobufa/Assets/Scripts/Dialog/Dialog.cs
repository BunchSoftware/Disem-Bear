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
    public DropEnum enterDrop = DropEnum.DropRight;
    public DropEnum exitDrop = DropEnum.DropLeft;
    public float speedText = 0.05f;
    [JsonIgnore] public Color colorText = Color.black;
    [HideInInspector] public string jsonHTMLColorRGBA;
    public FontStyle fontStyleText = FontStyle.Normal;
    public int fontSizeText = 40;
    public bool skipDialog = false;
    public bool stopTheEndDialog = false;
    [TextArea(10, 100)]
    public string textDialog = "";
    public float waitSecond = 0;
}
