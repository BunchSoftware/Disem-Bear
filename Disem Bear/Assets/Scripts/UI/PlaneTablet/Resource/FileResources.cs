using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FileResources", menuName = "FileResources")]
public class FileResources : ScriptableObject
{
    public List<ResourceData> resources;
}
