using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.Environment
{
    public class RayCastManager : MonoBehaviour
    {
        private List<ClickableObject> clickables = new();
        private GameObject currentClickObject = null;
        private GameObject currentStayObject = null;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                LeftMouseClick();
            }
            MouseOnSomething();
        }

        private void MouseOnSomething()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, 100f, LayerMask.GetMask("ClickedObject")))
            {
                currentStayObject = infoHit.collider.gameObject;
                if (currentStayObject.TryGetComponent<ClickableObject>(out var obj))
                {
                    if (!clickables.Contains(obj))
                    {
                        clickables.Add(obj);
                    }
                }

            }
            foreach (ClickableObject obj in clickables)
            {
                obj.MouseStayOnObjectFunction(currentStayObject);
            }
        }

        private void LeftMouseClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var infoHit, 100f, LayerMask.GetMask("ClickedObject")))
            {
                currentClickObject = infoHit.collider.gameObject;
                if (currentClickObject.TryGetComponent<ClickableObject>(out var obj))
                {
                    if (!clickables.Contains(obj))
                    {
                        clickables.Add(obj);
                    }
                }

            }
            foreach (ClickableObject obj in clickables)
            {
                obj.MouseClickObjectFunction(currentClickObject);
            }
        }
    }
}

