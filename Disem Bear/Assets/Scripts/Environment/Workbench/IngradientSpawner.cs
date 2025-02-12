using Game.Environment.LModelBoard;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Game.Environment.LMixTable
{
    public class IngradientSpawner : MonoBehaviour, ILeftMouseUpClickable, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        [SerializeField] private Ingradient ingradient;

        public UnityEvent<Ingradient> OnPickUpIngradient;
        public UnityEvent<Ingradient> OnPutIngradient;

        private SpriteRenderer spriteRenderer;
        private Workbench workbench;
        private TriggerObject triggerObject;

        private bool isEndDrag = false;

        private IngradientCell pointer;

        private Collider spawnerCollider;

        private Collider dragCollider;
        private Bounds dragBounds;

        private Collider contentCollider;
        private Bounds contentBounds;

        public void Init(Workbench workbench, TriggerObject triggerObject)
        {
            this.workbench = workbench;
            this.triggerObject = triggerObject;

            contentCollider = transform.parent.GetComponent<Collider>();
            contentBounds = contentCollider.bounds;

            spawnerCollider = GetComponent<Collider>();

            spriteRenderer = GetComponent<SpriteRenderer>();

            if (ingradient.typeIngradient == string.Empty)
                Debug.LogError($"Не задан тип инградиента {name}");

            CheckCountIngradient();

            workbench.OnStartWorkbenchClose.AddListener(() =>
            {
                if (workbench.IsOpen && pointer != null)
                    workbench.DropIngradient(this);
            });
        }

        private void CheckCountIngradient()
        {
            if (ingradient.countIngradient == 0)
            {
                spawnerCollider.enabled = false;
                spriteRenderer.enabled = false;
            }
            else if(ingradient.countIngradient > 0)
            {
                spawnerCollider.enabled = true;
                spriteRenderer.enabled = true;
            }
        }

        public Ingradient PickUpIngradient(int countIngradient)
        {
            Ingradient ingradientOut = null;

            if (this.ingradient.countIngradient > 0)
            {
                ingradientOut = new Ingradient();
                ingradientOut.typeIngradient = ingradient.typeIngradient;
                ingradientOut.countIngradient = countIngradient;

                ingradient.countIngradient = (int)Mathf.Clamp(ingradient.countIngradient - countIngradient, 0, int.MaxValue);

                CheckCountIngradient();

                OnPickUpIngradient?.Invoke(ingradientOut);
            }

            return ingradientOut;
        }

        public void PutIngradient(int countIngradient)
        {
            Ingradient ingradientOut = new Ingradient();
            ingradientOut.typeIngradient = ingradient.typeIngradient;
            ingradientOut.countIngradient = countIngradient;

            ingradient.countIngradient = (int)Mathf.Clamp(ingradient.countIngradient + countIngradient, 0, int.MaxValue);

            CheckCountIngradient();

            OnPutIngradient?.Invoke(ingradientOut);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && ingradient.countIngradient != 0)
            {
                pointer = workbench.DragIngradient(this);

                dragCollider = pointer.GetComponent<Collider>();
                dragBounds = dragCollider.bounds;

                if ((int)Mathf.Clamp(ingradient.countIngradient - Workbench.countIngradiensTaken, 0, int.MaxValue) == 0)
                {
                    spawnerCollider.enabled = false;
                    spriteRenderer.enabled = false;
                }
                else
                { 
                    spawnerCollider.enabled = true;
                    spriteRenderer.enabled = true;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && ingradient.countIngradient != 0 && workbench.IsOpen)
            {
                Vector3 positionCursor = ScreenPositionInWorldPosition.GetWorldPositionOnPlaneXZ(Input.mousePosition, transform.position.y);

                Vector3 position = new Vector3();

                position.x = Math.Clamp(positionCursor.x, contentBounds.min.x + dragBounds.size.x / 2, contentBounds.max.x - dragBounds.size.x / 2);
                position.z = Math.Clamp(positionCursor.z, contentBounds.min.z + dragBounds.size.z / 2, contentBounds.max.z - dragBounds.size.z / 2);

                pointer.transform.position =
                    new Vector3(
                        position.x,
                        transform.position.y,
                        position.z
                    );
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (workbench.IsOpen)
            {
                workbench.DropIngradient(this);
                CheckCountIngradient();

                pointer = null;
                dragCollider = null;
            }
        }

        public Sprite GetSpriteIngradient()
        {
            return spriteRenderer.sprite;
        }


        public Ingradient GetIngradient()
        {
            return ingradient;
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
                if (!workbench.IsDrag && workbench.IsOpen && ingradient.countIngradient > 0)
                {
                    workbench.DropIngradient(this);
                    CheckCountIngradient();
                }
            }
        }

        public void OnMouseLeftClickUpOtherObject()
        {
            
        }
    }
}
