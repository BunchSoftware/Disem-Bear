using Game.Environment.Item;
using Game.Environment.LPostTube;
using Game.LPlayer;
using Unity.VisualScripting;
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

        private bool isClick = false;
        private bool isEndDrag = false;

        private GameObject pointerDrag;

        public void Init(Workbench workbench, TriggerObject triggerObject)
        {
            this.workbench = workbench;
            this.triggerObject = triggerObject;

            spriteRenderer = GetComponent<SpriteRenderer>();

            if (ingradient.typeIngradient == string.Empty)
                Debug.LogError($"Не задан тип инградиента {name}");

            if (ingradient.countIngradient == 0)
                spriteRenderer.gameObject.SetActive(false);


            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (!workbench.IsDrag && isClick && workbench.IsOpen && ingradient.countIngradient > 0)
                {
                    isClick = false;
                }
                else if (!workbench.IsDrag && workbench.IsOpen && ingradient.countIngradient > 0)
                {

                }
            });
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

                if(ingradient.countIngradient == 0)
                    spriteRenderer.enabled = false;

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

            if (ingradient.countIngradient > 0)
                spriteRenderer.enabled = true;

            OnPutIngradient?.Invoke(ingradientOut);
        }

        public Ingradient GetIngradient()
        {
            return ingradient;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && ingradient.countIngradient != 0)
            {
                pointerDrag = Instantiate(new GameObject(), transform.parent);
                SpriteRenderer spriteRenderer = pointerDrag.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = this.spriteRenderer.sprite;

                workbench.DragIngradient(this, pointerDrag);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && ingradient.countIngradient != 0 && workbench.IsOpen)
            {
                float distancePlane = Vector3.Distance(workbench.transform.position, Camera.main.transform.position);
                Vector3 position = Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + distancePlane)
                );

                pointerDrag.transform.position =
                    new Vector3(
                        pointerDrag.transform.position.x,
                        position.y,
                        position.z
                    );
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (workbench.IsOpen)
            {
                workbench.DropIngradient(this);
                pointerDrag = null;
            }
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
                isClick = true;
        }

        public void OnMouseLeftClickUpOtherObject()
        {
            isClick = false;
        }
    }
}
