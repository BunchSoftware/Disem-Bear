using External.DI;
using System;
using UnityEngine;

namespace Game.Environment
{
    public interface ILeftMouseClickable
    {
        void OnMouseLeftClickObject();
        void OnMouseLeftClickOtherObject();
    }

    public interface IMouseOver
    {
        void OnMouseEnterObject();
        void OnMouseExitObject();
    }

    [Serializable]
    public class PlayerInput : IUpdateListener
    {
        private ILeftMouseClickable currentLeftMouseClickable;
        private IMouseOver currentMouseOver;

        public void OnUpdate(float deltaTime)
        {
            if (Input.GetMouseButtonDown(0))
                LeftMouseClick();

            MouseOver();
        }

        private void LeftMouseClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100f);
            if (raycastHits.Length != 0)
            {
                for (int i = 0; i < raycastHits.Length; i++)
                {
                    ILeftMouseClickable leftMouseClickable;
                    if (raycastHits[i].collider.gameObject.TryGetComponent(out leftMouseClickable))
                    {
                        if (currentLeftMouseClickable != null)
                            currentLeftMouseClickable.OnMouseLeftClickOtherObject();

                        currentLeftMouseClickable = leftMouseClickable;
                        currentLeftMouseClickable.OnMouseLeftClickObject();

                        return;
                    }
                }

                if (currentLeftMouseClickable != null)
                      currentLeftMouseClickable.OnMouseLeftClickOtherObject();
            }
        }

        private void MouseOver()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100f);
            if (raycastHits.Length != 0)
            {
                for (int i = 0; i < raycastHits.Length; i++)
                {
                    IMouseOver mouseOver;
                    if (raycastHits[i].collider.gameObject.TryGetComponent(out mouseOver))
                    {
                        currentMouseOver = mouseOver;
                        currentMouseOver.OnMouseEnterObject();

                        return;
                    }
                }

                if (currentMouseOver != null)
                    currentMouseOver.OnMouseExitObject();
            }
        }
    }
}

