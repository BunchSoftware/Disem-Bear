using System.Collections.Generic;
using UnityEngine;


namespace Game.Environment.Fridge
{
    [CreateAssetMenu(fileName = "New Magnets", menuName = "Magnets")]
    public class MagnetDatabase : ScriptableObject
    {
        public List<Magnet> magnets = new List<Magnet>();
    }
}
