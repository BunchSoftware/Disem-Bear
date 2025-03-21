using Game.Environment;
using Game.Environment.LMixTable;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngradientDragBase : MonoBehaviour, ILeftMouseUpClickable, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    protected Workbench workbench;
    protected string nameeIngradient;

    protected Collider dragCollider;
    protected Bounds dragBounds;

    protected Collider dragFreeCollider;
    protected Bounds dragFreeBounds;

    public void Init(IngradientData ingradient, Workbench workbench, Collider dragFreeCollider)
    {
        this.workbench = workbench;
        this.dragFreeCollider = dragFreeCollider;

        dragFreeBounds = dragFreeCollider.bounds;
        dragCollider = GetComponent<Collider>();
        dragBounds = dragCollider.bounds;

        nameeIngradient = ingradient.typeIngradient;
    }

    public virtual void OnBeginDrag(PointerEventData eventData) { }

    public virtual void OnDrag(PointerEventData eventData) { }

    public virtual void OnEndDrag(PointerEventData eventData) { }

    public string GetNameIngradient()
    {
        return nameeIngradient;
    }

    public virtual void OnMouseLeftClickUpObject() { }

    public virtual void OnMouseLeftClickUpOtherObject() { }
}
