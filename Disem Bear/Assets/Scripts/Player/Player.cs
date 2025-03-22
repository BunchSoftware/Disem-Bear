using External.DI;
using External.Storage;
using Game.Environment;
using Game.Environment.Item;
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


        public void Init()
        {
            PickUpItem pickUpItem = GameBootstrap.FindPickUpItemToPrefabs(SaveManager.playerDatabase.JSONPlayer.resources.currentPickUpItem.namePickUpItem);

            if (pickUpItem != null)
            {
                this.pickUpItem = Instantiate(pickUpItem);

                if (this.pickUpItem.TryGetComponent(out ScaleChooseObject scaleChooseObject))
                    scaleChooseObject.RemoveComponent();

                this.pickUpItem.transform.parent = transform;

                playerPickUpItem = true;
                this.pickUpItem.CanTakeByCollisionPlayer = false;

                typePickUpItem = this.pickUpItem.TypeItem;
            }

            Debug.Log("Player: Успешно иницилизирован");
        }

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

                Debug.Log($"Игрок поднял предмет: {pickUpItem.name} {pickUpItem.NameItem}");

                this.pickUpItem = pickUpItem;
                if (this.pickUpItem.TryGetComponent(out ScaleChooseObject scaleChooseObject))
                    scaleChooseObject.RemoveComponent();
                this.pickUpItem.transform.parent = transform;

                SaveManager.playerDatabase.JSONPlayer.resources.currentPickUpItem.namePickUpItem = this.pickUpItem.NameItem;

                typePickUpItem = pickUpItem.TypeItem;
                OnPickUpItem?.Invoke(pickUpItem);

                SaveManager.UpdatePlayerDatabase();
            }
        }

        public PickUpItem PutItem()
        {
            PickUpItem temp = null;
            if (playerPickUpItem)
            {
                playerPickUpItem = false;
                OnPutItem?.Invoke(pickUpItem);

                Debug.Log($"Игрок положил предмет: {pickUpItem.name} {pickUpItem.NameItem}");

                temp = pickUpItem;
                pickUpItem = null;

                SaveManager.playerDatabase.JSONPlayer.resources.currentPickUpItem.namePickUpItem = null;
                SaveManager.UpdatePlayerDatabase();
                typePickUpItem = TypePickUpItem.None;
            }

            return temp;
        }

        public void EnterSomething(MonoBehaviour context)
        {
            playerInSomething = true;
            Debug.Log($"Игрок вошел в управление обьектом {context.name}");
            OnEnterSomething?.Invoke();
        }

        public void ExitSomething(MonoBehaviour context)
        {
            playerInSomething = false;
            Debug.Log($"Игрок вышел из управления обьектом {context.name}");
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
