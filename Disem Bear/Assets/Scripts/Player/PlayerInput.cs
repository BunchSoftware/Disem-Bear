using External.DI;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<ILeftMouseUpClickable> currentLeftMouseUpClickable = new List<ILeftMouseUpClickable>();
        private List<ILeftMouseClickable> currentLeftMouseClickable = new List<ILeftMouseClickable>();
        private List<ILeftMouseDownClickable> currentLeftMouseDownClickable = new List<ILeftMouseDownClickable>();

        private List<IRightMouseUpClickable> currentRightMouseUpClickable = new List<IRightMouseUpClickable>();
        private List<IRightMouseClickable> currentRightMouseClickable = new List<IRightMouseClickable>();
        private List<IRightMouseDownClickable> currentRightMouseDownClickable = new List<IRightMouseDownClickable>();

        private List<IMouseOver> currentMouseOver = new List<IMouseOver>();
        private GameObject overObject;

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
                List<ILeftMouseUpClickable> leftMouseUpClickable = hitInfo.collider.gameObject.GetComponents<ILeftMouseUpClickable>().ToList();

                if (leftMouseUpClickable != null && leftMouseUpClickable.Count > 0)
                {
                    if (currentLeftMouseUpClickable.Count > 0)
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

            if (currentLeftMouseUpClickable.Count > 0)
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
                List<ILeftMouseClickable> leftMouseClickable = hitInfo.collider.gameObject.GetComponents<ILeftMouseClickable>().ToList();

                if (leftMouseClickable != null && leftMouseClickable.Count > 0)
                {
                    if (currentLeftMouseClickable.Count > 0)
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

            if (currentLeftMouseClickable.Count > 0)
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
                List<ILeftMouseDownClickable> leftMouseDownClickable = hitInfo.collider.gameObject.GetComponents<ILeftMouseDownClickable>().ToList();

                if (leftMouseDownClickable != null && leftMouseDownClickable.Count > 0)
                {
                    if (currentLeftMouseDownClickable.Count > 0)
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

            if (currentLeftMouseDownClickable.Count > 0)
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
                List<IRightMouseUpClickable> rightMouseUpClickable = hitInfo.collider.gameObject.GetComponents<IRightMouseUpClickable>().ToList();
                if (rightMouseUpClickable != null && rightMouseUpClickable.Count > 0)
                {
                    if (currentRightMouseUpClickable.Count > 0)
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

            if (currentRightMouseClickable.Count > 0)
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
                List<IRightMouseClickable> rightMouseClickable = hitInfo.collider.gameObject.GetComponents<IRightMouseClickable>().ToList();
                if (rightMouseClickable != null && rightMouseClickable.Count > 0)
                {
                    if (currentRightMouseClickable.Count > 0)
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

            if (currentRightMouseClickable.Count > 0)
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
                List<IRightMouseDownClickable> rightMouseDownClickable = hitInfo.collider.gameObject.GetComponents<IRightMouseDownClickable>().ToList();
                if (rightMouseDownClickable != null && rightMouseDownClickable.Count > 0)
                {
                    if (currentRightMouseDownClickable.Count > 0)
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

            if (currentRightMouseDownClickable.Count > 0)
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
                List<IMouseOver> mouseOver = hitInfo.collider.gameObject.GetComponents<IMouseOver>().ToList();

                if (overObject == null && hitInfo.collider.gameObject.TryGetComponent<IMouseOver>(out var temp))
                {
                    overObject = hitInfo.collider.gameObject;
                    currentMouseOver = mouseOver;

                    foreach (var obj in currentMouseOver)
                    {
                        obj.OnMouseEnterObject();
                    }
                }
                
                if (overObject != null && overObject != hitInfo.collider.gameObject)
                {
                    overObject = null;
                    foreach (var obj in currentMouseOver)
                    {
                        obj.OnMouseExitObject();
                    }
                    currentMouseOver.Clear();
                }
            }
        }
    }
}

