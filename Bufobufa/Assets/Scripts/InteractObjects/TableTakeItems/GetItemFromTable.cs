using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItemFromTable : MonoBehaviour
{
    private GameObject Player;
    private bool InTrigger = false;
    private bool ClickedMouse = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit))
            {
                if (infoHit.collider.gameObject == gameObject)
                {
                    ClickedMouse = true;
                }
                else
                {
                    ClickedMouse = false;
                }
            }
        }

    }
}
