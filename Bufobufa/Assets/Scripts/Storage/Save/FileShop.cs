using External.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace External.Storage
{
    [CreateAssetMenu(fileName = "New FileShop", menuName = "FileShop")]
    public class FileShop : ScriptableObject
    {
        public JSONShop JSONShop;
    }
}
