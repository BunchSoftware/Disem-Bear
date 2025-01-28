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

    public interface IRightMouseClickable
    {
        void OnMouseRightClickObject();
        void OnMouseRightClickOtherObject();
    }

    public interface IMouseOver
    {
        void OnMouseEnterObject();
        void OnMouseExitObject();
    }

    [Serializable]
    public class PlayerInput : IUpdateListener
    {
        private ILeftMouseClickable[] currentLeftMouseClickable = new ILeftMouseClickable[0];
        private IRightMouseClickable[] currentRightMouseClickable = new IRightMouseClickable[0];
        private IMouseOver[] currentMouseOver = new IMouseOver[0];

        public void OnUpdate(float deltaTime)
        {
            if (Input.GetMouseButtonDown(0))
                LeftMouseClick();
            if (Input.GetMouseButtonDown(1))
                RightMouseClick();

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
                    ILeftMouseClickable[] leftMouseClickable = raycastHits[i].collider.gameObject.GetComponents<ILeftMouseClickable>();
                    if (leftMouseClickable.Length > 0)
                    {
                        if (currentLeftMouseClickable.Length > 0)
                        {
                            foreach (var obj in currentLeftMouseClickable)
                            {
                                obj.OnMouseLeftClickOtherObject();

                            }
                        }

                        currentLeftMouseClickable = leftMouseClickable;
                        foreach (var obj in currentLeftMouseClickable)
                        {
                            obj.OnMouseLeftClickObject();
                        }


                        return;
                    }
                }

                if (currentLeftMouseClickable != null)
                {
                    foreach (var obj in currentLeftMouseClickable)
                    {
                        obj.OnMouseLeftClickOtherObject();
                    }
                }
            }
        }

        private void RightMouseClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100f);
            if (raycastHits.Length != 0)
            {
                for (int i = 0; i < raycastHits.Length; i++)
                {
                    IRightMouseClickable[] rightMouseClickable = raycastHits[i].collider.gameObject.GetComponents<IRightMouseClickable>();
                    if (rightMouseClickable.Length > 0)
                    {
                        if (currentRightMouseClickable.Length > 0)
                        {
                            foreach (var obj in currentRightMouseClickable)
                            {
                                obj.OnMouseRightClickOtherObject();
                            }
                        }

                        currentRightMouseClickable = rightMouseClickable;
                        foreach (var obj in currentRightMouseClickable)
                        {
                            obj.OnMouseRightClickObject();
                        }


                        return;
                    }
                }

                if (currentRightMouseClickable != null)
                {
                    foreach (var obj in currentRightMouseClickable)
                    {
                        obj.OnMouseRightClickOtherObject();
                    }
                }
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
                    IMouseOver[] mouseOver = raycastHits[i].collider.gameObject.GetComponents<IMouseOver>();
                    if (mouseOver.Length > 0)
                    {
                        if (currentMouseOver.Length > 0 && currentMouseOver != mouseOver)
                        {
                            foreach(var obj in currentMouseOver)
                            {
                                obj.OnMouseExitObject();
                            }
                        }

                        currentMouseOver = mouseOver;
                        foreach (var obj in currentMouseOver)
                        {
                            obj.OnMouseEnterObject();
                        }

                        return;
                    }
                }

                if (currentMouseOver.Length > 0)
                {
                    foreach (var obj in currentMouseOver)
                    {
                        obj.OnMouseExitObject();
                    }
                }
            }
        }
    }
}

