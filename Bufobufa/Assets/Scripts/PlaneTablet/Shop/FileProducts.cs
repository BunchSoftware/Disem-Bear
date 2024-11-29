using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Product", menuName = "Product")]
public class FileProducts : ScriptableObject
{
    public List<Product> products = new List<Product>();
}
