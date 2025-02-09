using External.Storage;
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
        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;
        private TriggerObject triggerObject;

        public UnityEvent OnAquariumOpen;
        public UnityEvent OnAquariumClose;

        private SaveManager saveManager;
        private Player player;
        private PlayerMouseMove playerMouseMove;
        private MovePointToPoint spriteMovePointToPoint;




        public void Init(SaveManager saveManager, Player player, PlayerMouseMove playerMouseMove)
        {
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();
            triggerObject = GetComponentInChildren<TriggerObject>();
            spriteMovePointToPoint = transform.Find("AquariumSprite").GetComponent<MovePointToPoint>();

            openObject.OnStartObjectOpen.AddListener(() =>
            {
                spriteMovePointToPoint.StartMoveTo(openObject.timeOpen);
                scaleChooseObject.on = false;
            });
            openObject.OnEndObjectOpen.AddListener(() =>
            {
                
                OnAquariumOpen?.Invoke();
            });

            openObject.OnStartObjectClose.AddListener(() =>
            {
                spriteMovePointToPoint.StartMoveTo(openObject.timeClose);
            });
            openObject.OnEndObjectClose.AddListener(() =>
            {
                scaleChooseObject.on = true;
                OnAquariumClose?.Invoke();
            });
            openObject.Init(triggerObject, playerMouseMove, player);

        }



        public void OnUpdate(float deltaTime)
        {
            openObject.OnUpdate(deltaTime);
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

        }

        public void OnMouseLeftClickDownOtherObject()
        {

        }
    }
}
