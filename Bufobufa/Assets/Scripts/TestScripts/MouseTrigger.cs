using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrigger : MonoBehaviour
{
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        if (!GetComponent<TableOpen>().TableIsOpen)
            transform.localScale = originalScale * 1.08f; // Увеличиваем объект
        else transform.localScale = originalScale;
    }

    private void OnMouseExit()
    {
        transform.localScale = originalScale;
    }
}