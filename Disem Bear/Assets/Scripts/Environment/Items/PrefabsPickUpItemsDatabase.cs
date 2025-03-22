using Game.Environment.Item;
using System.Collections.Generic;
using UnityEngine;

namespace External.Storage
{
    [CreateAssetMenu(fileName = "New FilePrefabsPickUpItems", menuName = "FilePrefabsPickUpItems")]
    public class PrefabsPickUpItemsDatabase : ScriptableObject
    {
        public List<PickUpItem> pickUpItems;
    }
}
