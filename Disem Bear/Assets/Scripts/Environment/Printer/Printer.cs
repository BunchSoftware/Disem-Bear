using Game.Environment.Item;
using Game.LPlayer;
using Game.Music;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.Printer
{
    public class Printer : MonoBehaviour, ILeftMouseDownClickable
    {
        public bool IsPrinterWork => isPrinterWork;
        private bool isPrinterWork = false;

        [SerializeField] private List<PrinterObjectInfo> printerObjectInfos = new();

        [SerializeField] private TriggerObject triggerObject;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private MeshRenderer printerRenderer;
        [SerializeField] private Material materialDonePrinter;
        [SerializeField] private AudioClip soundPrinter;

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        public UnityEvent<PickUpItem> OnStartWork;
        public UnityEvent<PickUpItem> OnEndWork;

        private Material materialPrinter;
        private Player player;
        private SoundManager soundManager;
        private Animator animator;
        private bool isClick = false;
        private PrinterObjectInfo outPrinterObjectInfo;

        public void Init(SoundManager soundManager, Player player)
        {
            animator = GetComponent<Animator>();
            materialPrinter = printerRenderer.material;

            this.soundManager = soundManager;
            this.player = player;

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (isClick)
                {
                    isClick = false;
                    if (!isPrinterWork && player.PlayerPickUpItem && !player.PlayerInSomething)
                    {
                        for (int i = 0; i < printerObjectInfos.Count; i++)
                        {
                            if (printerObjectInfos[i].inputItem.NameItem == player.GetPickUpItem().NameItem)
                            {
                                isPrinterWork = true;

                                PickUpItem pickUpItem = player.PutItem();
                                Destroy(pickUpItem.gameObject);

                                soundManager.OnPlayOneShot(soundPrinter);
                                animator.SetInteger("State", 1);
                                particle.Play();

                                StartCoroutine(WaitWhilePrintObject(printerObjectInfos[i]));

                                break;
                            }
                        }
                    }
                    else if (isPrinterWork && !player.PlayerPickUpItem)
                    {
                        printerRenderer.GetComponent<MeshRenderer>().material = materialPrinter;

                        PickUpItem pickUpItem = Instantiate(outPrinterObjectInfo.outItem);
                        OnPickUpItem?.Invoke(pickUpItem);
                        player.PickUpItem(pickUpItem);

                        outPrinterObjectInfo = null;
                        isPrinterWork = false;
                    }
                }
            });

            Debug.Log("Printer: Успешно иницилизирован");
        }

        private IEnumerator WaitWhilePrintObject(PrinterObjectInfo printerObjectInfo)
        {
            OnStartWork?.Invoke(printerObjectInfo.outItem);
            OnPutItem?.Invoke(printerObjectInfo.outItem);

            yield return new WaitForSeconds(printerObjectInfo.timePrint);

            outPrinterObjectInfo = printerObjectInfo;

            animator.SetInteger("State", 0);
            printerRenderer.GetComponent<MeshRenderer>().material = materialDonePrinter;
            particle.Stop();

            OnEndWork?.Invoke(printerObjectInfo.outItem);
        }

        public void OnMouseLeftClickDownObject()
        {
            isClick = true;
        }

        public void OnMouseLeftClickDownOtherObject()
        {
            isClick = false;
        }


        [System.Serializable]
        public class PrinterObjectInfo
        {
            public PickUpItem inputItem;
            public float timePrint;
            public PickUpItem outItem;
        }
    }
}
