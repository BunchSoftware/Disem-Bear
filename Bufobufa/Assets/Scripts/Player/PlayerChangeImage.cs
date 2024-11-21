using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeImage : MonoBehaviour
{
    private Vector3 LastPos;
    private SpriteRenderer spriteRender;
    [SerializeField] private Sprite Left;
    [SerializeField] private Sprite Right;
    [SerializeField] private Sprite Forward;
    [SerializeField] private Sprite Back;
    private void Start()
    {
        LastPos = transform.position;
        spriteRender = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (LastPos.x - transform.position.x > 0.015f)
        {
            spriteRender.sprite = Left;
        }
        else if (transform.position.x - LastPos.x > 0.015f)
        {
            spriteRender.sprite = Right;
        }
        else if (transform.position.z <= LastPos.z)
        {
            spriteRender.sprite = Forward;
        }
        else if (transform.position.z > LastPos.z)
        {
            spriteRender.sprite = Back;
        }
        
        
        LastPos = transform.position;
    }
}
