using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment
{
    [RequireComponent(typeof(Collider))]
    public class TriggerObject : MonoBehaviour
    {
        [SerializeField] private string tagObject;
        public UnityEvent<Collider> OnTriggerEnterEvent;
        public UnityEvent<Collider> OnTriggerStayEvent;
        public UnityEvent<Collider> OnTriggerExitEvent;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == tagObject)
                OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == tagObject)
                OnTriggerStayEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == tagObject)
                OnTriggerExitEvent?.Invoke(other);
        }
    }
}
