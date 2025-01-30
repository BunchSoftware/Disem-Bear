using External.Storage;
using Game.Environment;
using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.Environment.LPostTube;
using Game.Environment.LTableWithItems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.LPlayer
{
    public class Player : MonoBehaviour
    {
        public bool PlayerInSomething => playerInSomething;
        private bool playerInSomething = false;

        public bool PlayerPickUpItem => playerPickUpItem;
        private bool playerPickUpItem = false;

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        public UnityEvent OnEnterSomething;
        public UnityEvent OnExitSomething;

        private PickUpItem pickUpItem;
        private TypePickUpItem typePickUpItem = TypePickUpItem.None;

        [SerializeField] private ParticleSystem playerParticleSystem;
        public ParticleSystem PlayerParticleSystem => playerParticleSystem;

        [SerializeField] private GameObject pointItemLeft;
        public GameObject PointItemLeft => pointItemLeft;

        [SerializeField] private GameObject pointItemRight;
        public GameObject PointItemRight => pointItemRight;

        [SerializeField] private GameObject pointItemBack;
        public GameObject PointItemBack => pointItemBack;

        [SerializeField] private GameObject pointItemForward;
        public GameObject PointItemForward => pointItemForward;


        private void OnCollisionEnter(Collision collision)
        {
            if (playerPickUpItem == false)
            {
                PickUpItem pickUpItem;

                if (collision.collider.TryGetComponent<PickUpItem>(out pickUpItem) && pickUpItem.CanTakeByCollisionPlayer)
                    PickUpItem(pickUpItem);
            }
        }

        public void PickUpItem(PickUpItem pickUpItem)
        {
            if (playerPickUpItem == false && pickUpItem != null)
            {
                playerPickUpItem = true;
                pickUpItem.CanTakeByCollisionPlayer = false;

                this.pickUpItem = pickUpItem;
                if (this.pickUpItem.TryGetComponent(out ScaleChooseObject scaleChooseObject))
                    scaleChooseObject.RemoveComponent();
                this.pickUpItem.transform.parent = transform;

                typePickUpItem = pickUpItem.TypeItem;
                OnPickUpItem?.Invoke(pickUpItem);
            }
        }

        public PickUpItem PutItem()
        {
            if (playerPickUpItem)
            {
                playerPickUpItem = false;
                OnPutItem?.Invoke(pickUpItem);
                pickUpItem = null;
                typePickUpItem = TypePickUpItem.None;
            }

            return pickUpItem;
        }

        public void EnterSomething()
        {
            playerInSomething = true;
            OnEnterSomething?.Invoke();
        }

        public void ExitSomething()
        {
            playerInSomething = false;
            OnExitSomething?.Invoke();
        }

        public PickUpItem GetPickUpItem()
        {
            return pickUpItem;
        }

        public TypePickUpItem GetTypePickUpItem()
        {
            return typePickUpItem;
        }
    }
}
