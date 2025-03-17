using External.Storage;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Environment.Fridge
{
    public class Magnet : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public bool IsDrag => isDrag;
        private bool isDrag = false;

        public UnityEvent OnDragMagnet;
        public UnityEvent OnDropMagnet;

        private Collider contentColider;
        private Bounds dragBounds;

        private Collider magnetCollider;
        private Bounds magnetBounds;

        private SpriteRenderer spriteRenderer;
        private MagnetInfo magnetInfo;

        private Fridge fridge;
        private int indexMagnet = 0;

        public void Init(Fridge fridge, int indexMagnet, MagnetInfo magnetInfo, Bounds dragBounds)
        {
           this.magnetInfo = magnetInfo;
           this.fridge = fridge;
           this.dragBounds = dragBounds;
           this.indexMagnet = indexMagnet;
           contentColider = transform.parent.GetComponent<Collider>();

           magnetCollider = transform.GetComponent<Collider>();
           magnetBounds = magnetCollider.bounds;

           spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = this.magnetInfo.iconMagnet;

            transform.localPosition = new Vector3(this.magnetInfo.x, this.magnetInfo.y, this.magnetInfo.z);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if(fridge.IsOpen && eventData.button == PointerEventData.InputButton.Left)
            {
                isDrag = true;
                OnDragMagnet?.Invoke();
                fridge.SortOrderMagnets(indexMagnet);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (fridge.IsOpen && eventData.button == PointerEventData.InputButton.Left)
            {
                float distancePlane = Vector3.Distance(contentColider.transform.position, Camera.main.transform.position);

                Vector3 positionCursor = Camera.main.ScreenToWorldPoint(
                                   new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + distancePlane)
                               );

                Vector3 position = new Vector3();

                position.x = Math.Clamp(positionCursor.x, dragBounds.min.x + magnetBounds.size.x / 2, dragBounds.max.x - magnetBounds.size.x / 2);
                position.y = Math.Clamp(positionCursor.y, dragBounds.min.y + magnetBounds.size.y / 2, dragBounds.max.y - magnetBounds.size.y / 2);
                position.z = Math.Clamp(positionCursor.z, dragBounds.min.z + magnetBounds.size.z / 2, dragBounds.max.z - magnetBounds.size.z / 2);


                transform.position = position;
                transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);

                MagnetData magnetSave = new MagnetData();
                magnetSave.typeMagnet = magnetInfo.typeMagnet;
                magnetSave.x = transform.localPosition.x;
                magnetSave.y = transform.localPosition.y;
                magnetSave.z = transform.localPosition.z;

                SaveManager.ChangeMagnetSave(magnetSave);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (fridge.IsOpen)
            {
                isDrag = false;
                OnDropMagnet?.Invoke();
            }
        }


        public MagnetInfo GetMagnet()
        {
            return magnetInfo;
        }
    }
}