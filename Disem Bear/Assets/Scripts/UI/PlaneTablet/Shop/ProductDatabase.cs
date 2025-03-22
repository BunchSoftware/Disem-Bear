using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Shop
{
    [CreateAssetMenu(fileName = "New Product", menuName = "Product")]
    public class ProductDatabase : ScriptableObject
    {
        public List<Product> products = new List<Product>();
    }
}
