using External.DI;
using Game.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Game.Environment
{
    [RequireComponent(typeof(ClickableObject))]
    public class ScaleChooseObject : MonoBehaviour
    {
        public bool on = true;
        public float coefficient = 1.08f;

        private Vector3 maxScale;
        private Vector3 minScale;

        private ClickableObject clickableObject;

        private void Start()
        {
            clickableObject = GetComponent<ClickableObject>();
            minScale = transform.localScale;
            maxScale = new Vector3(minScale.x * coefficient, minScale.y * coefficient, minScale.z * coefficient);
        }


        private void Update()
        {
            if (on && clickableObject.MouseStayOnObject && transform.localScale.x < maxScale.x)
            {
                Vector3 scale = new Vector3(transform.localScale.x + Time.deltaTime * minScale.x, transform.localScale.y + Time.deltaTime * minScale.y, transform.localScale.z + Time.deltaTime * minScale.z);
                if (scale.x > maxScale.x)
                {
                    scale = maxScale;
                }
                transform.localScale = scale;
            }
            else if (transform.localScale.x > minScale.x && (!on || !clickableObject.MouseStayOnObject))
            {
                Vector3 scale = new Vector3(transform.localScale.x - Time.deltaTime * minScale.x, transform.localScale.y - Time.deltaTime * minScale.y, transform.localScale.z - Time.deltaTime * minScale.z);
                if (scale.x < minScale.x)
                {
                    scale = minScale;
                }
                transform.localScale = scale;
            }
        }

        public void RemoveComponent()
        {
            transform.localScale = minScale;
            Destroy(this);
        }
    }
}
