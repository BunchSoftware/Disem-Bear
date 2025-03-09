using External.API;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace External.Storage
{
    [CreateAssetMenu(fileName = "New FilePlayer", menuName = "FilePlayer")]
    public class FilePlayer : ScriptableObject
    {
        public JSONPlayer JSONPlayer;

        public JSONPlayer Clone()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<FilePlayer>(serialized).JSONPlayer;
        }
    }
}
