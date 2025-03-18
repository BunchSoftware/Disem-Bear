using External.Storage;
using Game.Environment.Item;
using Game.Environment.LPostTube;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Environment.Aquarium
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class AquariumOpen : MonoBehaviour, ILeftMouseDownClickable
    {
        [SerializeField] private Aquarium aquarium;
        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;
        [SerializeField] private TriggerObject triggerObject;
        private Collider collider;

        public UnityEvent OnAquariumOpen;
        public UnityEvent OnAquariumClose;

        private Player player;
        private PlayerMouseMove playerMouseMove;
        private MovePointToPoint spriteMovePointToPoint;

        private bool isClick = false;
        private bool isOpen = false;




        public void Init(Player player, PlayerMouseMove playerMouseMove)
        {
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();
            if (triggerObject == null)
                Debug.LogError("Не задан триггер у аквариума");
            spriteMovePointToPoint = transform.Find("AquariumSprite").GetComponent<MovePointToPoint>();
            collider = GetComponent<Collider>();

            openObject.OnStartObjectOpen.AddListener(() =>
            {
                isOpen = true;
                spriteMovePointToPoint.StartMoveTo(openObject.timeOpen);
                scaleChooseObject.on = false;
            });
            openObject.OnEndObjectOpen.AddListener(() =>
            {
                collider.enabled = false;
                OnAquariumOpen?.Invoke();
            });

            openObject.OnStartObjectClose.AddListener(() =>
            {
                collider.enabled = true;
                spriteMovePointToPoint.StartMoveTo(openObject.timeClose);
            });
            openObject.OnEndObjectClose.AddListener(() =>
            {
                isOpen = false;
                scaleChooseObject.on = true;
                OnAquariumClose?.Invoke();
            });
            openObject.Init(triggerObject, playerMouseMove, player);
            aquarium.Init(player);

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (isClick && !isOpen)
                {
                    isClick = false;

                    if (player.PlayerPickUpItem && TryGetMaterial(player.GetPickUpItem()))
                    {
                        Destroy(player.GetPickUpItem().gameObject);
                        player.PutItem();
                        Debug.Log("Material for aquarium update");
                    }
                }
            });

            Debug.Log("AquariumOpen: Успешно иницилизирован");
        }



        public void OnUpdate(float deltaTime)
        {
            if (openObject != null)
                openObject.OnUpdate(deltaTime);
            if (aquarium != null)
                aquarium.OnUpdate(deltaTime);
        }

        private bool TryGetMaterial(PickUpItem pickUpItem)
        {
            if (pickUpItem != null)
            {
                MaterialForAquarium materialForAquarium;
                switch (pickUpItem.TypeItem)
                {
                    case TypePickUpItem.None:
                        break;
                    case TypePickUpItem.AquariumMaterial:
                        
                        if (pickUpItem.TryGetComponent(out materialForAquarium))
                        {
                            aquarium.UpdateMaterial(materialForAquarium);
                            return true;
                        }
                        else
                        {
                            Debug.LogError("Объект задан как материал аквариума, но не имеет скрипта MaterialForAquarium");
                        }
                        break;
                    case TypePickUpItem.Package:
                        PackageItem packageItem;
                        if (pickUpItem.TryGetComponent(out packageItem))
                        {
                            if (packageItem.itemInPackage.TryGetComponent(out materialForAquarium))
                            {
                                aquarium.UpdateMaterial(materialForAquarium);
                                return true;
                            }
                            else
                            {
                                Debug.Log("Отказ в принятии посылки, так как нет скрипта MaterialForAquarium");
                            }
                        }
                        else
                        {
                            Debug.LogError("Ошибка. На обьекте нет PackageItem, но обьект указан как Package");
                        }
                        break;
                }
            }
            return false;
        }

        //private void Start()
        //{
        //    Player = GameObject.FindGameObjectWithTag("Player");
        //    AquariumSprite = transform.Find("AquariumSprite").gameObject;
        //}
        private void Update()
        {
            //if (GetComponent<OpenObject>().ObjectIsOpen && OneTap)
            //{
            //    OneTap = false;
            //    AquariumSprite.GetComponent<MoveAnimation>().startCoords = CoordAquarium;
            //    AquariumSprite.GetComponent<MoveAnimation>().needPosition = true;
            //    AquariumSprite.GetComponent<MoveAnimation>().startRotate = RotateAquarium;
            //    AquariumSprite.GetComponent<MoveAnimation>().needRotate = true;
            //    AquariumSprite.GetComponent<MoveAnimation>().TimeAnimation = TimeAnimationAquarium;
            //    AquariumSprite.GetComponent<MoveAnimation>().StartMove();

            //    AquariumSprite.GetComponent<BoxCollider>().enabled = true;
            //    Temperature.GetComponent<BoxCollider>().enabled = true;
            //}
            //else if (!GetComponent<OpenObject>().ObjectIsOpen && !OneTap)
            //{
            //    OneTap = true;
            //    AquariumSprite.GetComponent<MoveAnimation>().EndMove();

            //    AquariumSprite.GetComponent<BoxCollider>().enabled = false;
            //    Temperature.GetComponent<BoxCollider>().enabled = false;


            //}
            //else if (Player.GetComponent<Player>().PlayerPickUpItem && !GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().InTrigger && GetComponent<OpenObject>().ClickedMouse && !GetComponent<OpenObject>().ObjectIsOpen)
            //{
            //    //GetComponent<OpenObject>().InTrigger = false;
            //    GetComponent<OpenObject>().ClickedMouse = false;
            //    //if (Player.GetComponent<Player>().currentPickObject.GetComponent<MaterialForAquarium>())
            //    //{
            //    //    if (Player.GetComponent<Player>().currentPickObject.GetComponent<MaterialForAquarium>().nameMaterial == AquariumSprite.GetComponent<Aquarium>().NameMaterial)
            //    //    {
            //    //        AquariumSprite.GetComponent<Aquarium>().TimeWaterSpend = Player.GetComponent<Player>().currentPickObject.GetComponent<MaterialForAquarium>().TimeMaterial;
            //    //        AquariumSprite.GetComponent<Aquarium>().OnAquarium = true;
            //    //        Player.GetComponent<Player>().PutItem();
            //    //        Destroy(Player.GetComponent<Player>().currentPickObject);
            //    //        Player.GetComponent<Player>().currentPickObject = null;
            //    //    }
            //    //}
            //}
        }

        public void OnMouseLeftClickDownObject()
        {
            isClick = true;
        }

        public void OnMouseLeftClickDownOtherObject()
        {
            isClick = false;
        }
    }
}
