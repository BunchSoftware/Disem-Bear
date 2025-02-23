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
        [Header("PriceChangeProduct")]
        public string typePriceChangeProduct;
        public int countPriceChange;
        public Sprite avatarPriceChange;
    }
}
