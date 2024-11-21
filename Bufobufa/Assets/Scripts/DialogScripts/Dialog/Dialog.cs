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
    public DropEnum enterDrop;
    public DropEnum exitDrop;
    public float speedText = 0.05f;
    public Color colorText = Color.white; 
    public FontStyle fontStyleText;
    public bool skipDialog = false;
    public bool stopTheEndDialog = false;
    public string textDialog;
    public float waitSecond = 0;
}
