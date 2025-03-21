using Game.Environment.LMixTable;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngradientDragObject : IngradientDragBase
{

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && workbench.IsOpen)
        {
            workbench.DragIngradient(this);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && workbench.IsOpen)
        {
            Vector3 positionCursor = ScreenPositionInWorldPosition.GetWorldPositionOnPlaneXZ(Input.mousePosition, transform.position.y);

            Vector3 position = new Vector3();

            position.x = Math.Clamp(positionCursor.x, dragFreeBounds.min.x, dragFreeBounds.max.x);
            position.z = Math.Clamp(positionCursor.z, dragFreeBounds.min.z, dragFreeBounds.max.z);

            transform.position =
                new Vector3(
                    position.x,
                    transform.position.y,
                    position.z
                );
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        workbench.DropIngradient(this);
    }

    public override void OnMouseLeftClickUpObject()
    {
        if (!workbench.IsDrag && !workbench.IsEndDrag && workbench.IsOpen)
            workbench.DropIngradient(this);
    }
}
