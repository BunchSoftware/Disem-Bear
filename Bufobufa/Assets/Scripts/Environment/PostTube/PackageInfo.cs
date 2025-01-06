using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment.PostTube
{
    public class PackageInfo : MonoBehaviour
    {
        public string PackageName;
        public GameObject ItemInPackage;
        public bool HaveIngredients = false;
        public string NameIngredient = "";
        public int amount = 0;
    }
}
