using External.DI;
using Game.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Game.Environment
{
    public class ScaleChooseObject : MonoBehaviour, IMouseOver
    {
        public bool on = true;
        public float coefficient = 1.08f;

        private Vector3 maxScale;
        private Vector3 minScale;
        private bool increaseScale = false;

        private void Start()
        {
            minScale = transform.localScale;
            maxScale = new Vector3(minScale.x * coefficient, minScale.y * coefficient, minScale.z * coefficient);
        }


        private void Update()
        {
            if (on && increaseScale && transform.localScale.x < maxScale.x)
            {
                Vector3 scale = new Vector3(transform.localScale.x + Time.deltaTime * minScale.x, transform.localScale.y + Time.deltaTime * minScale.y, transform.localScale.z + Time.deltaTime * minScale.z);
                if (scale.x > maxScale.x)
                {
                    scale = maxScale;
                }
                transform.localScale = scale;
            }
            else if (transform.localScale.x > minScale.x && (!increaseScale || !on))
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

        public void OnMouseEnterObject()
        {
            increaseScale = true;
        }

        public void OnMouseExitObject()
        {
            increaseScale = false;
        }
    }
}
