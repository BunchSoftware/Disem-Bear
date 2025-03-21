using Game.Environment.Item;
using Game.Environment.LMixTable;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New CraftRecipe", menuName = "CraftRecipe")]
public class CraftRecipe : ScriptableObject
{
    public List<IngradientData> inputIngradients;
    public List<IngradientData> outIngradients;
    public List<PickUpItem> outPickUpItems;
}
