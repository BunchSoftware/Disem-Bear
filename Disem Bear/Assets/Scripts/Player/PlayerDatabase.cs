using External.API;
using UnityEngine;

namespace External.Storage
{
    [CreateAssetMenu(fileName = "New FilePlayer", menuName = "FilePlayer")]
    public class PlayerDatabase : ScriptableObject
    {
        public JSONPlayer JSONPlayer;
    }
}
