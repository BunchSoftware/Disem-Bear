using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Shop
{
    [Serializable]
    public class Product
    {
        [HideInInspector] public int indexProduct;
        public string header;
        public Reward reward;
        [Header("PriceProduct")]
        public string typePriceProduct;
        public int price = 1;
        public Sprite avatarPriceProduct;
    }
}
