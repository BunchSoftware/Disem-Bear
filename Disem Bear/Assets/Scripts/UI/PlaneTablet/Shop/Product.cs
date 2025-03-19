using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Shop
{
    [Serializable]
    public class Product
    {
        public string header;
        public float durationOfAppearance = -1;
        [HideInInspector] public bool isVisible = true;
        public Reward reward;
        [Header("PriceProduct")]
        public string typePriceProduct;
        public int price = 1;
        public Sprite avatarPriceProduct;
        public string description = "";
    }
}
