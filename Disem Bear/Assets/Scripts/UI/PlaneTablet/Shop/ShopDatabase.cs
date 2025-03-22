using External.API;
using UnityEngine;

namespace External.Storage
{
    [CreateAssetMenu(fileName = "New FileShop", menuName = "FileShop")]
    public class ShopDatabase : ScriptableObject
    {
        public JSONShop JSONShop;
    }
}
