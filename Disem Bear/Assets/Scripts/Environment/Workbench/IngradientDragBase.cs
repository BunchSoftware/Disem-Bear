using Game.Environment;
using Game.Environment.LMixTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngradientDragBase : MonoBehaviour, ILeftMouseUpClickable, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    protected Workbench workbench;
    protected string typeIngradient;

    protected Collider dragCollider;
    protected Bounds dragBounds;

    protected Collider dragFreeCollider;
    protected Bounds dragFreeBounds;

    public void Init(Ingradient ingradient, Workbench workbench, Collider dragFreeCollider)
    {
        this.workbench = workbench;
        this.dragFreeCollider = dragFreeCollider;

        dragFreeBounds = dragFreeCollider.bounds;
        dragCollider = GetComponent<Collider>();
        dragBounds = dragCollider.bounds;

        typeIngradient = ingradient.typeIngradient;
    }

    public virtual void OnBeginDrag(PointerEventData eventData) { }

    public virtual void OnDrag(PointerEventData eventData) { }

    public virtual void OnEndDrag(PointerEventData eventData) { }

    public string GetTypeIngradient()
    {
        return typeIngradient;
    }

    public virtual void OnMouseLeftClickUpObject() { }

    public virtual void OnMouseLeftClickUpOtherObject() { }
}
