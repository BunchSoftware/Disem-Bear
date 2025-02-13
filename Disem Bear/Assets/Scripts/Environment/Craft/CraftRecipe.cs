using Game.Environment.LMixTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New CraftRecipe", menuName = "CraftRecipe")]
public class CraftRecipe : ScriptableObject
{
    public List<Ingradient> inputIngradients;
    public List<Ingradient> outIngradients;
}
