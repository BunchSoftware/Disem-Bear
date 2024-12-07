using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetGUI : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Magnet magnet;

    public void Init(Magnet magnet)
    {
        this.magnet = magnet;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = magnet.iconMagnet;

        transform.localPosition = new Vector3(magnet.x, magnet.y, magnet.z);
    }

    public Magnet GetMagnet()
    {
        return magnet;
    }
}
