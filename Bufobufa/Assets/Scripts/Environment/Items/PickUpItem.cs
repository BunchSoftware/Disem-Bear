using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Game.Environment.Item
{
    public class PickUpItem : MonoBehaviour
    {
        public string TypeItem => typeItem;
        [SerializeField] private string typeItem;
        public bool falling = true;
        private Vector3 lcScale = new();
        public bool PickUp = false;
        private bool InTrigger = false;

        private void Start()
        {
            StartCoroutine(NotFalling());
        }
        private void Update()
        {
            if (!falling && !PickUp && InTrigger)
            {
                GetComponent<MouseTrigger>().enabled = false;
                PickUp = true;
                GetComponent<BoxCollider>().enabled = false;
                //Player.GetComponent<Player>().PickSomething();
                //Player.GetComponent<Player>().currentPickObject = gameObject;
                Destroy(this);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                InTrigger = false;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                InTrigger = true;
            }
        }



        IEnumerator NotFalling()
        {
            yield return new WaitForSeconds(1f);

            falling = false;
        }
    }
}
