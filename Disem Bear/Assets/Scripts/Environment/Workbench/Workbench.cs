using External.Storage;
using Game.Environment.Item;
using Game.Environment.LModelBoard;
using Game.LPlayer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Environment.LMixTable
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class Workbench : MonoBehaviour
    {
        [SerializeField] private TriggerObject triggerObject;
        [SerializeField] private List<IngradientSpawner> ingradientSpawners;
        [SerializeField] private Transform mixTable;

        public UnityEvent OnStartWorkbenchOpen;
        public UnityEvent OnEndWorkbenchOpen;

        public UnityEvent OnStartWorkbenchClose;
        public UnityEvent OnEndWorkbenchClose;

        public UnityEvent<Ingradient> OnDragIngradient;
        public UnityEvent<Ingradient> OnDropIngradient;

        private SaveManager saveManager;
        private Player player;
        private PlayerMouseMove playerMouseMove;

        private Collider colliderWorkbench;
        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;

        public bool IsOpen => isOpen;
        private bool isOpen = false;

        public bool IsDrag => isDrag;
        private bool isDrag = false;

        private GameObject pointerDrag;

        public void Init(SaveManager saveManager, Player player, PlayerMouseMove playerMouseMove)
        {
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            colliderWorkbench = GetComponent<Collider>();
            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                ingradientSpawners[i].Init(this, triggerObject);
            }

            openObject.OnStartObjectOpen.AddListener(() =>
            {
                scaleChooseObject.on = false;
                colliderWorkbench.enabled = false;
                OnStartWorkbenchOpen?.Invoke();
            });
            openObject.OnEndObjectOpen.AddListener(() =>
            {
                isOpen = true;
                OnEndWorkbenchOpen?.Invoke();
            });
            openObject.OnStartObjectClose.AddListener(() =>
            {
                OnStartWorkbenchClose?.Invoke();
                isOpen = false;
            });
            openObject.OnEndObjectClose.AddListener(() =>
            {
                colliderWorkbench.enabled = true;
                scaleChooseObject.on = true;
                OnEndWorkbenchClose?.Invoke();
            });
            openObject.Init(triggerObject, playerMouseMove, player);
        }

        public void DragIngradient(IngradientSpawner ingradientSpawner, GameObject pointerDrag)
        {
            this.pointerDrag = pointerDrag;
            if (ingradientSpawner.GetIngradient().countIngradient > 0)
            {
                isDrag = true;
                OnDragIngradient?.Invoke(ingradientSpawner.GetIngradient());

                for (int i = 0; i < ingradientSpawners.Count; i++)
                {
                    ingradientSpawners[i].GetComponent<Collider>().enabled = false;
                }
            }
        }

        private bool InMixTable()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 100f;

            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxDistance);

            for (int i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].collider.gameObject.transform == mixTable)
                {
                    return true;
                }
            }

            return false;
        }

        public void DropIngradient(IngradientSpawner ingradientSpawner)
        {
            Ingradient ingradient = ingradientSpawner.PickUpIngradient(1);
            if (pointerDrag != null && ingradient != null)
            {
                if(InMixTable())
                {
                    ingradientSpawner.EndDrag();
                }
                else
                {
                    ingradientSpawner.PutIngradient(1);
                    ingradientSpawner.EndDrag();
                    Destroy(pointerDrag.gameObject);
                }

                pointerDrag = null;
            }

            isDrag = false;
            OnDropIngradient?.Invoke(ingradient);

            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                ingradientSpawners[i].GetComponent<Collider>().enabled = true;
            }
        }

        public void OnUpdate(float deltaTime)
        {
            openObject.OnUpdate(deltaTime);

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
