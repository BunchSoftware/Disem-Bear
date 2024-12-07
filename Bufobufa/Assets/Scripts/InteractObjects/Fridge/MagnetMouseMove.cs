using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetMouseMove : MonoBehaviour
{
    public bool OnDrag = false;

    private Transform FrontFridge;


    private void Start()
    {
        FrontFridge = transform.parent.Find("FrontFridge");
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("ClickedObject")))
            {
                if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("Magnet")) && infoHit.transform == transform)
                {
                    OnDrag = true;
                    transform.parent.GetComponent<FridgeOpen>().ChangeMouseTrigger();
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        if (OnDrag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, Mathf.Infinity, LayerMask.GetMask("ClickedObject")) && !Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("MagnetCollider")) && !Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("DoorHandle")))
            {
                if (infoHit.transform == FrontFridge)
                {
                    transform.position = new Vector3(infoHit.point.x, infoHit.point.y, infoHit.point.z);// + offset;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnDrag = false;
            transform.parent.GetComponent<FridgeOpen>().OnMouseTrigger();
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}