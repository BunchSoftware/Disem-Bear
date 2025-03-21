using External.API;
using UnityEngine;

namespace External.Storage
{
    [CreateAssetMenu(fileName = "New FileShop", menuName = "FileShop")]
    public class FileShop : ScriptableObject
    {
        public JSONShop JSONShop;
    }
}
