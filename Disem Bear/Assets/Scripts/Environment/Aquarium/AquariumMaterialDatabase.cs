using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Environment.Aquarium;
using UnityEngine;

namespace Assets.Scripts.Environment.Aquarium
{
    [CreateAssetMenu(fileName = "AquariumMaterialDatabase", menuName = "AquariumMaterialDatabase")]
    class AquariumMaterialDatabase : ScriptableObject
    {
        public List<MaterialForAquarium> materialForAquariums = new List<MaterialForAquarium>();
    }
}
