using System.Collections.Generic;
using UnityEngine;


namespace Game.Environment.Fridge
{
    [CreateAssetMenu(fileName = "New Magnets", menuName = "Magnets")]
    public class FileMagnets : ScriptableObject
    {
        public List<MagnetInfo> magnets = new List<MagnetInfo>();
    }
}
