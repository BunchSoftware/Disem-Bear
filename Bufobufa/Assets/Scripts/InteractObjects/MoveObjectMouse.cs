using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveObjectMouse : MonoBehaviour
{
    public bool OnDrag = false;

    private Vector3 offset;

    public GameObject wtf;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("ClickedObject")))
            {
                wtf = infoHit.transform.gameObject;
                if (infoHit.transform == transform)
                {
                    if (Physics.Raycast(ray, out infoHit, Mathf.Infinity, LayerMask.GetMask("Table")))
                    {
                        offset = transform.position - new Vector3(infoHit.point.x, transform.position.y, infoHit.point.z);
                        OnDrag = true;
                    }
                }
            }
        }
        if (OnDrag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("Table")))
            {
                transform.position = new Vector3(infoHit.point.x, transform.position.y, infoHit.point.z) + offset;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnDrag = false;
        }
    }
}
