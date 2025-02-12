using Game.Environment;
using Game.Environment.LMixTable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngradientCell : MonoBehaviour, ILeftMouseUpClickable, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private Workbench workbench;
    private string typeIngradient;
    private bool isEndDrag = false;

    private Collider dragCollider;
    private Bounds dragBounds;

    private Collider contentCollider;
    private Bounds contentBounds;

    public void Init(Ingradient ingradient, Workbench workbench, Collider contentCollider)
    {
        this.workbench = workbench;
        this.contentCollider = contentCollider;

        contentBounds = contentCollider.bounds;
        dragCollider = GetComponent<Collider>();
        dragBounds = dragCollider.bounds;

        typeIngradient = ingradient.typeIngradient;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && workbench.IsOpen)
        {
            workbench.DragIngradient(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && workbench.IsOpen)
        {
            Vector3 positionCursor = ScreenPositionInWorldPosition.GetWorldPositionOnPlaneXZ(Input.mousePosition, transform.position.y);

            Vector3 position = new Vector3();

            position.x = Math.Clamp(positionCursor.x, contentBounds.min.x + dragBounds.size.x / 2, contentBounds.max.x - dragBounds.size.x / 2);
            position.z = Math.Clamp(positionCursor.z, contentBounds.min.z + dragBounds.size.z / 2, contentBounds.max.z - dragBounds.size.z / 2);

            transform.position =
                new Vector3(
                    position.x,
                    transform.position.y,
                    position.z
                );
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        workbench.DropIngradient(this);
    }

    public void EndDrag()
    {
        isEndDrag = true;
    }

    public void OnMouseLeftClickUpObject()
    {
        if (isEndDrag)
            isEndDrag = false;
        else
        {
            if (!workbench.IsDrag && workbench.IsOpen)
            {
                workbench.DropIngradient(this);
                Debug.Log(12351);
            }
        }
    }

    public void OnMouseLeftClickUpOtherObject()
    {
        
    }

    public string GetTypeIngradient()
    {
        return typeIngradient;
    }
}
