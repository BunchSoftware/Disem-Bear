using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveObjectMouse : MonoBehaviour
{
    private Vector3 startPos;

    private Vector3 mousePosition;
    public bool OnDrag = false;

    private Vector3 target;
    private Vector3 offset;



    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePos();
        OnDrag = true;
    }
    private void OnMouseDrag()
    {
        //transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition).x, transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition).z);
    }
    private void OnMouseUp()
    {
        OnDrag = false;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("Table")))
            {
                target = new Vector3(infoHit.point.x, transform.position.y, infoHit.point.z);

            }
            offset = transform.position - target;
        }
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("Table")))
            {
                transform.position = new Vector3(infoHit.point.x, transform.position.y, infoHit.point.z) + offset;
            }
        }
    }
}
