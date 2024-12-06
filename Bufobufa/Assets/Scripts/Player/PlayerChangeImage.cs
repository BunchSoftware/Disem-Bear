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

    private Animator animator;
    private GameObject PointItemLeft;
    private GameObject PointItemRight;
    private GameObject PointItemBack;
    private GameObject PointItemForward;


    private float HorizontalChangePos;
    private float VerticalChangePos;
    private Sprite HorizontalSprite;
    private Sprite VerticalSprite;
    private string HorizontalAnimation;
    private string VerticalAnimation;
    private string CurrentAnimation = "StateAnimation";
    private void Start()
    {
        animator = GetComponent<Animator>();
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
        if (CurrentAnimation == "LeftAnimation")
        {
            GetComponent<PlayerInfo>().currentPickObject.transform.position = PointItemLeft.transform.position;
        }
        else if (CurrentAnimation == "RightAnimation")
        {
            GetComponent<PlayerInfo>().currentPickObject.transform.position = PointItemRight.transform.position;
        }
        else if (CurrentAnimation == "BackAnimation")
        {
            GetComponent<PlayerInfo>().currentPickObject.transform.position = PointItemBack.transform.position;
        }
        else if (CurrentAnimation == "ForwardAnimation" || CurrentAnimation == "StateAnimation")
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
            //HorizontalSprite = Left;
            //animator.Play("LeftAnimation");
            HorizontalAnimation = "LeftAnimation";
        }
        else
        {
            //HorizontalSprite = Right;
            //animator.Play("RightAnimation");
            HorizontalAnimation = "RightAnimation";
        }
        if (VerticalChangePos < -0.01f)
        {
            //VerticalSprite = Back;
            //animator.Play("BackAnimation");
            VerticalAnimation = "BackAnimation";
        }
        else
        {
            //VerticalSprite = Forward;
            //animator.Play("ForwardAnimation");
            VerticalAnimation = "ForwardAnimation";
        }
        if (Mathf.Abs(HorizontalChangePos) < 0.01f && Mathf.Abs(VerticalChangePos) < 0.01f)
        {
            CurrentAnimation = "StateAnimation";
        }
        else if (Mathf.Abs(HorizontalChangePos) > Mathf.Abs(VerticalChangePos))
        {
            CurrentAnimation = HorizontalAnimation;
        }
        else
        {
            CurrentAnimation = VerticalAnimation;
        }
        animator.Play(CurrentAnimation);
    }
}
