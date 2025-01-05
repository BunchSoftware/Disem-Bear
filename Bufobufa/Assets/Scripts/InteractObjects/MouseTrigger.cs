using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MouseTrigger : MonoBehaviour
{
    public bool AnyCase = false;
    private Vector3 originalScale;
    public bool OnScaleChange = false;
    public float TimeAnim = 0.2f;
    private float timer = 0f;
    private GameObject Player;
    private bool FinallyMove = false;
    public bool FullZero = true;

    private void Start()
    {
        originalScale = transform.localScale;
        Player = GameObject.Find("Player");
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("Floor", "ClickedObject")))
        {
            if (infoHit.collider.gameObject == gameObject)
            {
                if (AnyCase)
                {
                    if (FullZero)
                    {
                        FullZero = false;
                        originalScale = transform.localScale;
                    }
                    OnScaleChange = true;
                    FinallyMove = true;
                }
                else if (GetComponent<OpenObject>())
                {
                    if (!GetComponent<OpenObject>().ObjectIsOpen)
                    {
                        if (FullZero)
                        {
                            FullZero = false;
                            originalScale = transform.localScale;
                        }
                        OnScaleChange = true;
                        FinallyMove = true;
                    }
                }
                else
                {
                    if (!Player.GetComponent<PlayerInfo>().playerInSomething)
                    {
                        if (FullZero)
                        {
                            FullZero = false;
                            originalScale = transform.localScale;
                        }
                        OnScaleChange = true;
                        FinallyMove = true;
                    }
                }
            }
            else
            {
                OnScaleChange = false;
                FinallyMove = true;
            }
        }
        if (OnScaleChange)
        {
            if (timer <= TimeAnim)
            {
                transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.08f, timer / TimeAnim);
                timer += Time.deltaTime;
            }
            else if (FinallyMove)
            {
                transform.localScale = originalScale * 1.08f;
                FinallyMove = false;
            }
        }
        else
        {
            if (timer >= 0f)
            {
                transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.08f, timer / TimeAnim);
                timer -= Time.deltaTime;
            }
            else if (FinallyMove)
            {
                transform.localScale = originalScale;
                FinallyMove = false;
                FullZero = true;
            }
        }
    }
}