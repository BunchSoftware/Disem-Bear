using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Magnets", menuName = "Magnets")]
public class FileMagnets : ScriptableObject
{
    public List<Magnet> magnets = new List<Magnet>();
}
