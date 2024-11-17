using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererZ : MonoBehaviour
{
    public bool StaticObject = false;
    private void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -(int)(transform.position.z*1000);
        if (StaticObject) Destroy(this);
    }
    private void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -(int)(transform.position.z*1000);
    }
}
