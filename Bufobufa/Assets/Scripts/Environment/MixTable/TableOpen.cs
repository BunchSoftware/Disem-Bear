using External.Storage;
using Game.LPlayer;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Environment.LMixTable
{
    //[RequireComponent(typeof(OpenObject))]
    //[RequireComponent(typeof(ScaleChooseObject))]
    public class TableOpen : MonoBehaviour
    {
        private GameObject Player;
        private GameObject MixTable;

        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;
        [SerializeField] private TriggerObject triggerObject;
        private BoxCollider boxCollider;

        public UnityEvent OnTableOpen;
        public UnityEvent OnTableClose;

        private SaveManager saveManager;
        private Player player;
        private PlayerMouseMove playerMouseMove;



        public void Init(SaveManager saveManager, Player player, PlayerMouseMove playerMouseMove)
        {
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            boxCollider = GetComponent<BoxCollider>();
            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            openObject.OnEndObjectOpen.AddListener(() =>
            {
                boxCollider.enabled = false;
                scaleChooseObject.on = false;
                OnTableOpen?.Invoke();
            });
            openObject.OnEndObjectClose.AddListener(() =>
            {
                boxCollider.enabled = true;
                scaleChooseObject.on = true;
                OnTableClose?.Invoke();
            });
            openObject.Init(triggerObject, playerMouseMove, player);
        }

        public void OnUpdate(float deltaTime)
        {
            openObject.OnUpdate(deltaTime);
        }


        private void Start()
        {
            //Player = GameObject.FindGameObjectWithTag("Player");
            //MixTable = transform.Find("MixTable").gameObject;
        }
        private void Update()
        {
            //if (GetComponent<OpenObject>().ObjectIsOpen && !MixTable.GetComponent<ThingsInTableMix>().MixTableOn)
            //{
            //    MixTable.GetComponent<ThingsInTableMix>().MixTableOn = true;
            //}
            //else if (!GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().ObjectIsOpen && Input.GetMouseButtonDown(1))
            //{
            //    MixTable.GetComponent<ThingsInTableMix>().MixTableOn = false;
            //    if (MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject != null)
            //    {
            //        //Player.GetComponent<Player>().currentPickObject = MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject;
            //        MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject.transform.parent = Player.transform;
            //        MixTable.GetComponent<ThingsInTableMix>().currentPrinterObject = null;
            //       // Player.GetComponent<Player>().PickSomething();
            //    }
            //}
        }
    }
}
