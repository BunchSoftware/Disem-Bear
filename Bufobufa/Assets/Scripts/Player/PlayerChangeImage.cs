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
    private GameObject ParticleSystem;


    private float HorizontalChangePos;
    private float VerticalChangePos;
    private string HorizontalAnimation;
    private string VerticalAnimation;
    private string CurrentAnimation = "StateAnimation";
    private string CurrentAnimationParticle = "StateAnimation";
    private void Start()
    {
        ParticleSystem = transform.Find("Particle System").gameObject;
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
        ParticleSystemChange();
        if (GetComponent<PlayerInfo>().PlayerPickSometing)
            ItemInHandsChange();
        LastPos = transform.position;
    }
    private void ParticleSystemChange()
    {
        if (CurrentAnimation == "LeftAnimation" && CurrentAnimationParticle != "LeftAnimation")
        {
            CurrentAnimationParticle = "LeftAnimation";
            ParticleSystem.GetComponent<ParticleSystem>().Play();
            ParticleSystem.transform.eulerAngles = new Vector3(15.9691277f, 87.8853149f, 352.904419f);
        }
        else if (CurrentAnimation == "RightAnimation" && CurrentAnimationParticle != "RightAnimation")
        {
            CurrentAnimationParticle = "RightAnimation";
            ParticleSystem.GetComponent<ParticleSystem>().Play();
            ParticleSystem.transform.eulerAngles = new Vector3(15.9689913f, 272.11499f, 352.903992f);
        }
        else if (CurrentAnimation == "BackAnimation" && CurrentAnimationParticle != "BackAnimation")
        {
            CurrentAnimationParticle = "BackAnimation";
            ParticleSystem.GetComponent<ParticleSystem>().Play();
            ParticleSystem.transform.eulerAngles = new Vector3(15.9689932f, 0, 352.903992f);
        }
        else if (CurrentAnimation == "ForwardAnimation" && CurrentAnimationParticle != "ForwardAnimation")
        {
            CurrentAnimationParticle = "ForwardAnimation";
            ParticleSystem.GetComponent<ParticleSystem>().Play();
            ParticleSystem.transform.eulerAngles = new Vector3(15.9689932f, 180f, 352.903992f);
        }
        else if (CurrentAnimation == "StateAnimation" && CurrentAnimationParticle != "StateAnimation")
        {
            CurrentAnimationParticle = "StateAnimation";
            ParticleSystem.GetComponent<ParticleSystem>().Stop();
        }
    }
    private void ItemInHandsChange()
    {
        GetComponent<PlayerInfo>().currentPickObject.transform.localEulerAngles = Vector3.zero;
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
