using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Product 
{
    [HideInInspector] public int indexProduct;
    public string header;
    public string typeMachineDispensingProduct;
    [Header("ChangeProduct")]
    public string typeChangeProduct;
    public int countChangeProduct;
    public Sprite avatarChange;
    [Header("PriceChangeProduct")]
    public string typePriceChangeProduct;
    public int countPriceChange;
    public Sprite avatarPriceChange;
}
