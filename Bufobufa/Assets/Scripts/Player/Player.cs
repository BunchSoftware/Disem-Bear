using UnityEngine;
using UnityEngine.Events;

namespace Game.Player
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
            PickUpItem pickUpItem;
            CellTableWithItems cellTableWithItems;

            if (collision.collider.TryGetComponent<PickUpItem>(out pickUpItem))
            {
                PickUpItem(pickUpItem);
            }
            else if (collision.collider.TryGetComponent<CellTableWithItems>(out cellTableWithItems))
            {
                if (playerPickUpItem)
                    PickUpItem(cellTableWithItems.PickUpItem());
                else if (playerPickUpItem == false)
                {
                    if (cellTableWithItems.PutItem(this.pickUpItem))
                        PutItem();
                }
            }
        }

        public void PickUpItem(PickUpItem pickUpItem)
        {
            if (playerPickUpItem == false)
            {
                playerInSomething = true;

                this.pickUpItem = pickUpItem;
                this.pickUpItem.transform.parent = transform;

                OnPickUpItem?.Invoke(pickUpItem);
            }
        }

        public PickUpItem PutItem()
        {
            if (playerPickUpItem)
            {
                playerInSomething = false;
                pickUpItem = null;
                OnPutItem?.Invoke(pickUpItem);
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

        public PickUpItem GetPickUpObject()
        {
            return pickUpItem;
        }
    }
}
