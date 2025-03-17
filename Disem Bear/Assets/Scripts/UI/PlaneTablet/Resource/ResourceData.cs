using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceData 
{
    public string headerResource;
    public string typeResource;
    [HideInInspector] public int countResource;
    public Sprite iconResource;
}
