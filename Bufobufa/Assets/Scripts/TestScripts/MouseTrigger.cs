using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrigger : MonoBehaviour
{
    private Vector3 originalScale;
    private bool OnScaleChange = false;
    private bool OffScaleChange = false;
    public float TimeAnim = 0.2f;
    private float timer = 0f;
    private GameObject Player;

    private void Start()
    {
        originalScale = transform.localScale;
        Player = GameObject.Find("Player");
    }

    private void OnMouseEnter()
    {
        if (!Player.GetComponent<PlayerInfo>().PlayerInSomething)
        {
            OnScaleChange = true;
        }
    }

    private void OnMouseExit()
    {
        OnScaleChange = false;
    }
    private void Update()
    {
        if (OnScaleChange)
        {
            if (timer <= TimeAnim)
            {
                timer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.08f, timer / TimeAnim);
            }
        }
        else
        {
            if (timer >= 0f)
            {
                timer -= Time.deltaTime;
                transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.08f, timer / TimeAnim);
            }
        }
    }
}