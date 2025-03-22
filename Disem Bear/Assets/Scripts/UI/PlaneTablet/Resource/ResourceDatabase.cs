using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FileResources", menuName = "FileResources")]
public class ResourceDatabase : ScriptableObject
{
    public List<ResourceData> resources;
}
