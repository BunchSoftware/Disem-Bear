using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextRendererZ : MonoBehaviour
{
    public bool StaticObject = false;
    private void Start()
    {
        GetComponent<TextMeshPro>().sortingOrder = -(int)(transform.position.z * 1000);
        if (StaticObject) Destroy(this);
    }
    private void Update()
    {
        GetComponent<TextMeshPro>().sortingOrder = -(int)(transform.position.z * 1000);
    }
}
