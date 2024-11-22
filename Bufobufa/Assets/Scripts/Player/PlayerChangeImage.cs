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

    private GameObject PointItemLeft;
    private GameObject PointItemRight;
    private GameObject PointItemBack;
    private GameObject PointItemForward;


    private float HorizontalChangePos;
    private float VerticalChangePos;
    private Sprite HorizontalSprite;
    private Sprite VerticalSprite;
    private void Start()
    {
        LastPos = transform.position;
        spriteRender = GetComponent<SpriteRenderer>();
        PointItemLeft = transform.Find("PointItemLeft").gameObject;
        PointItemRight = transform.Find("PointItemRight").gameObject;
        PointItemBack = transform.Find("PointItemBack").gameObject;
        PointItemForward = transform.Find("PointItemForward").gameObject;
    }
    private void Update()
    {
        ImageChange();
        if (GetComponent<PlayerInfo>().PlayerPickSometing)
            ItemInHandsChange();
        LastPos = transform.position;
    }
    private void ItemInHandsChange()
    {
        if (spriteRender.sprite == Left)
        {
            GetComponent<PlayerInfo>().currentPickObject.transform.position = PointItemLeft.transform.position;
        }
        else if (spriteRender.sprite == Right)
        {
            GetComponent<PlayerInfo>().currentPickObject.transform.position = PointItemRight.transform.position;
        }
        else if (spriteRender.sprite == Back)
        {
            GetComponent<PlayerInfo>().currentPickObject.transform.position = PointItemBack.transform.position;
        }
        else if (spriteRender.sprite == Forward)
        {
            GetComponent<PlayerInfo>().currentPickObject.transform.position = PointItemForward.transform.position;
        }
    }
    private void ImageChange()
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
    }
}
