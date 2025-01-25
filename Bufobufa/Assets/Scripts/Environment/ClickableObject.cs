using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class ClickableObject : MonoBehaviour
    {
        public bool MouseClickObject = false;
        public bool MouseStayOnObject = false;

        public void MouseClickObjectFunction(GameObject obj)
        {
            if (gameObject == obj)
            {
                MouseClickObject = true;
            }
            else
            {
                MouseClickObject = false;
            }
        }
        public void MouseStayOnObjectFunction(GameObject obj)
        {
            if (gameObject == obj)
            {
                MouseStayOnObject = true;
            }
            else
            {
                MouseStayOnObject = false;
            }
        }
    }
}
