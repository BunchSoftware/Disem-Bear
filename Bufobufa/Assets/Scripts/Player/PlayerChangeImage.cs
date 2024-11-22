using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerChangeImage : MonoBehaviour
{
    private Vector3 LastPos;
    private SpriteRenderer spriteRender;
    [SerializeField] private Sprite Left;
    [SerializeField] private Sprite Right;
    [SerializeField] private Sprite Forward;
    [SerializeField] private Sprite Back;

    private float HorizontalChangePos;
    private float VerticalChangePos;
    private Sprite HorizontalSprite;
    private Sprite VerticalSprite;
    private void Start()
    {
        LastPos = transform.position;
        spriteRender = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        HorizontalChangePos = LastPos.x - transform.position.x;
        VerticalChangePos = LastPos.z - transform.position.z;
        if (HorizontalChangePos >= 0.01f)
        {
            HorizontalSprite = Left;
        }
        else
        {
            HorizontalSprite = Right;
        }
        if (VerticalChangePos < -0.01f)
        {
            VerticalSprite = Back;
        }
        else
        {
            VerticalSprite = Forward;
        }
        if (Mathf.Abs(HorizontalChangePos) > Mathf.Abs(VerticalChangePos))
        {
            spriteRender.sprite = HorizontalSprite;
        }
        else
        {
            spriteRender.sprite = VerticalSprite;
        }
        LastPos = transform.position;
    }
}
