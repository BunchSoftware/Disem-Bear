using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Environment.Aquarium
{
    [Serializable]
    public class MaterialForAquarium : MonoBehaviour
    {
        public List<string> cells = new();
        public string colorMaterial = "none";
        public float TimeMaterial = 10f;
    }
}
