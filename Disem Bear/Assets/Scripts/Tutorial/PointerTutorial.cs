using Game.Environment.Item;
using Game.LDialog;
using Game.LPlayer;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Tutorial
{
    public class PointerTutorial : MonoBehaviour
    {
        [Header("Dialog Settings")]
        [SerializeField] private int indexDialogPoint;
        [SerializeField] private int indexDialog;
        [Header("Pointer")]
        [SerializeField] private int indexPointer;
        [SerializeField] private float delayPointer = 0;
        [Header("Condition Pointer")]
        [SerializeField] private string conditionSkipDialog;
        [SerializeField] private PickUpItem pickUpItemPlayer;
        [Header("LineToObjectTransform")]
        public Transform lineToObjectTransform;

        public UnityEvent OnStartDialog;
        public UnityEvent SendInputFieldText;

        private DialogManager dialogManager;
        private PointerTutorialManager pointerManager;
        private Player player;

        public void Init(Player player, DialogManager dialogManager, PointerTutorialManager pointerManager)
        {
            this.dialogManager = dialogManager;
            this.pointerManager = pointerManager;
            this.player = player;   
            this.dialogManager.OnStartDialog.AddListener(SetPointer);
            this.dialogManager.SendInputFieldText.AddListener((message =>
            {
                SendInputFieldText?.Invoke();
            }));
            if(pickUpItemPlayer != null)
            {
                this.player.OnPickUpItem.AddListener((item) =>
                {
                    if (item.TypeItem == pickUpItemPlayer.TypeItem 
                    && item.NameItem == pickUpItemPlayer.NameItem)
                    {
                        dialogManager.RunConditionSkip(conditionSkipDialog);
                    }
                });
            }
        }

        public void SetPointer(Dialog dialog)
        {
            if (conditionSkipDialog.Length == 0 && dialogManager.GetCurrentIndexDialogPoint() == indexDialogPoint && dialogManager.GetCurrentIndexDialog() == indexDialog)
            {
                StartCoroutine(DelayPointer());
            }
        }

        private IEnumerator DelayPointer()
        {
            OnStartDialog?.Invoke();
            yield return new WaitForSeconds(delayPointer);
            pointerManager.SetPointer(indexPointer);
        }
    }
}
