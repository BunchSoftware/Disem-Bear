using UnityEngine;
using UnityEngine.Events;

namespace Game.Tutorial
{
    public class PointerTutorial : MonoBehaviour
    {
        [SerializeField] private int indexDialogPoint;
        [SerializeField] private int indexDialog;
        [SerializeField] private int indexPointer;
        [SerializeField] private string conditionSkipDialog;
        [SerializeField] private bool playerPickSomething;
        public UnityEvent SendInputFieldText;

        private DialogManager dialogManager;
        private PointerTutorialManager pointerManager;

        public void Init(DialogManager dialogManager, PointerTutorialManager pointerManager)
        {
            this.dialogManager = dialogManager;
            this.pointerManager = pointerManager;
            this.dialogManager.OnStartDialog.AddListener(SetPointer);
            this.dialogManager.SendInputFieldText.AddListener((message =>
            {
                SendInputFieldText?.Invoke();
            }));
        }

        public void SetPointer(Dialog dialog)
        {
            if (conditionSkipDialog == null && dialogManager.GetCurrentIndexDialogPoint() == indexDialogPoint && dialogManager.GetCurrentIndexDialog() == indexDialog)
                pointerManager.SetPointer(indexPointer);
        }

        public void RunConditionSkip()
        {
            dialogManager.RunConditionSkip(conditionSkipDialog);
        }
    }
}
