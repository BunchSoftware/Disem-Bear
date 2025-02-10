using External.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace External.Storage
{
    [CreateAssetMenu(fileName = "New FilePlayer", menuName = "FilePlayer")]
    public class FilePlayer : ScriptableObject
    {
        public JSONPlayer JSONPlayer;
    }
}
