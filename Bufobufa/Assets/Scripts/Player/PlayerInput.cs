using External.DI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Environment
{
    interface ILeftMouseUpClickable
    {
        void OnMouseLeftClickUpObject();
        void OnMouseLeftClickUpOtherObject();
    }

    interface ILeftMouseClickable
    {
        void OnMouseLeftClickObject();
        void OnMouseLeftClickOtherObject();
    }

    interface ILeftMouseDownClickable
    {
        void OnMouseLeftClickDownObject();
        void OnMouseLeftClickDownOtherObject();
    }

    public interface IRightMouseUpClickable
    {
        void OnMouseRightClickUpObject();
        void OnMouseRightClickUpOtherObject();
    }

    public interface IRightMouseClickable
    {
        void OnMouseRightClickObject();
        void OnMouseRightClickOtherObject();
    }

    public interface IRightMouseDownClickable
    {
        void OnMouseRightClickDownObject();
        void OnMouseRightClickDownOtherObject();
    }

    public interface IMouseOver
    {
        void OnMouseEnterObject();
        void OnMouseExitObject();
    }

    [Serializable]
    public class PlayerInput : IUpdateListener
    {
        private ILeftMouseUpClickable[] currentLeftMouseUpClickable = new ILeftMouseUpClickable[0];
        private ILeftMouseClickable[] currentLeftMouseClickable = new ILeftMouseClickable[0];
        private ILeftMouseDownClickable[] currentLeftMouseDownClickable = new ILeftMouseDownClickable[0];

        private IRightMouseUpClickable[] currentRightMouseUpClickable = new IRightMouseUpClickable[0];
        private IRightMouseClickable[] currentRightMouseClickable = new IRightMouseClickable[0];
        private IRightMouseDownClickable[] currentRightMouseDownClickable = new IRightMouseDownClickable[0];

        private IMouseOver[] currentMouseOver = new IMouseOver[0];

        public void OnUpdate(float deltaTime)
        {
            MouseOver();          

            if (Input.GetMouseButtonUp(0))
                LeftMouseUpClick();
            if (Input.GetMouseButtonUp(1))
                RightMouseUpClick();

            if (Input.GetMouseButton(0))
                LeftMouseClick();
            if (Input.GetMouseButton(1))
                RightMouseClick();

            if (Input.GetMouseButtonDown(0))
                LeftMouseDownClick();
            if (Input.GetMouseButtonDown(1))
                RightMouseDownClick();
        }
        #region LeftMouseClick

        private void LeftMouseUpClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;
            int layerMask = -1;
            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                ILeftMouseUpClickable[] leftMouseUpClickable = hitInfo.collider.gameObject.GetComponents<ILeftMouseUpClickable>();

                if (leftMouseUpClickable.Length > 0)
                {
                    if (currentLeftMouseUpClickable.Length > 0)
                    {
                        foreach (var obj in currentLeftMouseUpClickable)
                        {
                            obj.OnMouseLeftClickUpOtherObject();

                        }
                    }

                    currentLeftMouseUpClickable = leftMouseUpClickable;
                    foreach (var obj in currentLeftMouseUpClickable)
                    {
                        obj.OnMouseLeftClickUpObject();
                    }


                    return;
                }
            }

            if (currentLeftMouseUpClickable.Length > 0)
            {
                foreach (var obj in currentLeftMouseUpClickable)
                {
                    obj.OnMouseLeftClickUpOtherObject();
                }
            }
        }

        private void LeftMouseClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;
            int layerMask = -1;
            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                ILeftMouseClickable[] leftMouseClickable = hitInfo.collider.gameObject.GetComponents<ILeftMouseClickable>();
                
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

            if (currentLeftMouseClickable.Length > 0)
            {
                foreach (var obj in currentLeftMouseClickable)
                {
                    obj.OnMouseLeftClickOtherObject();
                }
            }
        }

        private void LeftMouseDownClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;
            int layerMask = -1;
            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                ILeftMouseDownClickable[] leftMouseDownClickable = hitInfo.collider.gameObject.GetComponents<ILeftMouseDownClickable>();

                if (leftMouseDownClickable.Length > 0)
                {
                    if (currentLeftMouseDownClickable.Length > 0)
                    {
                        foreach (var obj in currentLeftMouseDownClickable)
                        {
                            obj.OnMouseLeftClickDownOtherObject();

                        }
                    }

                    currentLeftMouseDownClickable = leftMouseDownClickable;
                    foreach (var obj in currentLeftMouseDownClickable)
                    {
                        obj.OnMouseLeftClickDownObject();
                    }


                    return;
                }
            }

            if (currentLeftMouseDownClickable.Length > 0)
            {
                foreach (var obj in currentLeftMouseDownClickable)
                {
                    obj.OnMouseLeftClickDownOtherObject();
                }
            }
        }

        #endregion LeftMouseClick

        #region RightMouseClick
        private void RightMouseUpClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;
            int layerMask = -1;
            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                IRightMouseUpClickable[] rightMouseUpClickable = hitInfo.collider.gameObject.GetComponents<IRightMouseUpClickable>();
                if (rightMouseUpClickable.Length > 0)
                {
                    if (currentRightMouseUpClickable.Length > 0)
                    {
                        foreach (var obj in currentRightMouseUpClickable)
                        {
                            obj.OnMouseRightClickUpOtherObject();
                        }
                    }

                    currentRightMouseUpClickable = rightMouseUpClickable;
                    foreach (var obj in currentRightMouseUpClickable)
                    {
                        obj.OnMouseRightClickUpObject();
                    }


                    return;
                }
            }

            if (currentRightMouseClickable.Length > 0)
            {
                foreach (var obj in currentRightMouseClickable)
                {
                    obj.OnMouseRightClickOtherObject();
                }
            }
        }

        private void RightMouseClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;
            int layerMask = -1;
            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                IRightMouseClickable[] rightMouseClickable = hitInfo.collider.gameObject.GetComponents<IRightMouseClickable>();
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

            if (currentRightMouseClickable.Length > 0)
            {
                foreach (var obj in currentRightMouseClickable)
                {
                    obj.OnMouseRightClickOtherObject();
                }
            }
        }

        private void RightMouseDownClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;
            int layerMask = -1;
            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                IRightMouseDownClickable[] rightMouseDownClickable = hitInfo.collider.gameObject.GetComponents<IRightMouseDownClickable>();
                if (rightMouseDownClickable.Length > 0)
                {
                    if (currentRightMouseDownClickable.Length > 0)
                    {
                        foreach (var obj in currentRightMouseDownClickable)
                        {
                            obj.OnMouseRightClickDownOtherObject();
                        }
                    }

                    currentRightMouseDownClickable = rightMouseDownClickable;
                    foreach (var obj in currentRightMouseDownClickable)
                    {
                        obj.OnMouseRightClickDownObject();
                    }


                    return;
                }
            }

            if (currentRightMouseDownClickable.Length > 0)
            {
                foreach (var obj in currentRightMouseDownClickable)
                {
                    obj.OnMouseRightClickDownOtherObject();
                }
            }
        }
        #endregion RightMouseClick

        private void MouseOver()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;
            int layerMask = -1;
            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                IMouseOver[] mouseOver = hitInfo.collider.gameObject.GetComponents<IMouseOver>();
                if (mouseOver.Length > 0)
                {
                    if (currentMouseOver.Length > 0 && currentMouseOver != mouseOver)
                    {
                        foreach (var obj in currentMouseOver)
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

