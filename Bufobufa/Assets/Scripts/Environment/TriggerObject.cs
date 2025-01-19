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
        public UnityEvent<Collider> OnTriggerExitEvent;

        private Collider triggerObject;

        private void Awake()
        {
            triggerObject = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == tagObject)
                OnTriggerEnterEvent?.Invoke(other);

            triggerObject.enabled = false;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == tagObject)
                OnTriggerExitEvent?.Invoke(other);

            triggerObject.enabled = true;
        }
    }
}
