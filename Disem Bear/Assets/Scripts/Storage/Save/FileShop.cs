using External.API;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace External.Storage
{
    [CreateAssetMenu(fileName = "New FileShop", menuName = "FileShop")]
    public class FileShop : ScriptableObject
    {
        public JSONShop JSONShop;

        public JSONShop Clone()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<FileShop>(serialized).JSONShop;
        }
    }
}
