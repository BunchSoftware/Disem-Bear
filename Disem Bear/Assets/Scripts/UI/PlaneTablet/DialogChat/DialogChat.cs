using Game.LDialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.PlaneTablet.DialogChat
{
    [Serializable]
    public class DialogChat
    {
        [SerializeField] private GameObject contentMessage;
        [SerializeField] private GameObject prefabMessage;
        private List<DialogMessageGroup> dialogMessageGroups = new List<DialogMessageGroup>();
        [SerializeField] private GameObject contentPanelChoices;
        [SerializeField] private GameObject prefabButtonChoice;
        private List<DialogChoiceButton> dialogChoiceButtons = new List<DialogChoiceButton>();
        [SerializeField] private DialogDatabase fileDialog;
        public UnityEvent<Dialog> EndDialog;

        private int currentIndexDialogPoint = 0;
        private int currentIndexDialog = 0;
        private DialogMessageGroup currentDialogMessageGroup;

        private bool isCanSkipDialog = false;

        private MonoBehaviour context;
        private Coroutine currentTypeLineCoroutine;

        public void Init(MonoBehaviour context)
        {
            this.context = context;
            contentPanelChoices.SetActive(false);

            Debug.Log("DialogChat: ������� ��������������");
        }

        public void StartDialog(int indexDialogPoint)
        {
            if (currentDialogMessageGroup != null)
                currentDialogMessageGroup.StopTypeLine();
            if (indexDialogPoint >= 0 && indexDialogPoint <= fileDialog.dialogPoints.Count)
            {
                currentIndexDialogPoint = indexDialogPoint;
                TypeLine(fileDialog.dialogPoints[indexDialogPoint], 0);
            }
            else
                Debug.LogError("������ ! ������ ������� ������� �� ����� ���������� �������� !");
        }
        public void SkipReplica()
        {
            if (isCanSkipDialog)
            {
                Dialog dialog = null;

                if (currentIndexDialog >= 0 && currentIndexDialog <= fileDialog.dialogPoints[currentIndexDialogPoint].dialog.Count)
                    dialog = fileDialog.dialogPoints[currentIndexDialogPoint].dialog[currentIndexDialog];

                if (dialog != null)
                {
                    StopTypeLine();
                    ExitDrop(dialog);
                    currentDialogMessageGroup.DialogFinish(dialog);
                    currentIndexDialog++;
                    TypeLine(fileDialog.dialogPoints[currentIndexDialogPoint], currentIndexDialog);
                }
            }
        }

        public void TypeLine(DialogPoint dialogPoint, int indexDialog)
        {
            if (currentTypeLineCoroutine == null)
                currentTypeLineCoroutine = context.StartCoroutine(TypeLineIE(dialogPoint, indexDialog));
        }

        private IEnumerator TypeLineIE(DialogPoint dialogPoint, int indexDialog)
        {
            currentIndexDialog = indexDialog;
            for (int i = currentIndexDialog; i < dialogPoint.dialog.Count; i++)
            {
                currentIndexDialog = i;
                isCanSkipDialog = true;
                contentPanelChoices.SetActive(false);

                foreach (Transform child in contentPanelChoices.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                if (dialogPoint.dialog[i].dialogChoices.Count >= 1)
                {
                    contentPanelChoices.SetActive(true);
                    for (int j = 0; j < dialogPoint.dialog[i].dialogChoices.Count; j++)
                    {
                        prefabButtonChoice.name = $"Button Choice {j}";
                        GameObject button = GameObject.Instantiate(prefabButtonChoice, contentPanelChoices.transform);
                        DialogChoiceButton dialogChoiceButton = button.GetComponent<DialogChoiceButton>();
                        dialogChoiceButton.Init((indexDialogPoint) =>
                        {
                            StartDialog(indexDialogPoint);
                        }, dialogPoint.dialog[i].dialogChoices[j]);
                        dialogChoiceButtons.Add(dialogChoiceButton);
                    }

                    EndDialog?.Invoke(dialogPoint.dialog[i]);
                }
                else
                {
                    prefabMessage.name = $"Message {dialogMessageGroups.Count}";
                    GameObject message = GameObject.Instantiate(prefabMessage, contentMessage.transform);
                    currentDialogMessageGroup = message.GetComponent<DialogMessageGroup>();

                    if (dialogPoint.dialog[i].enterDrop == DropEnum.DropLeft)
                        currentDialogMessageGroup.Init(this, DialogSide.Right);
                    else if (dialogPoint.dialog[i].enterDrop == DropEnum.DropRight)
                        currentDialogMessageGroup.Init(this, DialogSide.Left);
                    else
                        Debug.LogWarning("�������������� ! ��� �� ������������ �������� ����� ����");

                    dialogMessageGroups.Add(currentDialogMessageGroup);

                    EnterDrop(dialogPoint.dialog[i]);
                    currentDialogMessageGroup.StartTypeLine(dialogPoint.dialog[i]);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(contentMessage.GetComponent<RectTransform>());
                    yield return new WaitForSeconds(dialogPoint.dialog[i].speedText * dialogPoint.dialog[i].textDialog.Length);

                    EndDialog?.Invoke(dialogPoint.dialog[i]);

                    if (dialogPoint.dialog[i].stopTheEndDialog == true)
                    {
                        if (dialogPoint.dialog[i].skipDialogButton == false)
                        {
                            yield return new WaitForSeconds(dialogPoint.dialog[i].waitSecond);
                            ExitDrop(dialogPoint.dialog[i]);
                        }
                        break;
                    }
                    else
                        yield return new WaitForSeconds(dialogPoint.dialog[i].waitSecond);

                    ExitDrop(dialogPoint.dialog[i]);

                    isCanSkipDialog = false;
                }
            }
        }

        private void StopTypeLine()
        {
            if (currentTypeLineCoroutine != null)
                context.StopCoroutine(currentTypeLineCoroutine);
            currentDialogMessageGroup.StopTypeLine();
            isCanSkipDialog = false;
        }

        private void EnterDrop(Dialog dialog)
        {
            switch (dialog.enterDrop)
            {
                case DropEnum.DropRight:
                    {
                        currentDialogMessageGroup.currentMessage.animator.SetInteger("State", 1);
                    }
                    break;
                case DropEnum.DropLeft:
                    {
                        currentDialogMessageGroup.currentMessage.animator.SetInteger("State", 2);
                    }
                    break;
                case DropEnum.DropDown:
                    {
                        currentDialogMessageGroup.currentMessage.animator.SetInteger("State", 3);
                    }
                    break;

                case DropEnum.DropUp:
                    {
                        currentDialogMessageGroup.currentMessage.animator.SetInteger("State", 4);
                    }
                    break;
            }
        }
        private void ExitDrop(Dialog dialog)
        {
            switch (dialog.exitDrop)
            {
                case DropEnum.DropRight:
                    {
                        currentDialogMessageGroup.currentMessage.animator.SetInteger("State", 1);
                    }
                    break;
                case DropEnum.DropLeft:
                    {
                        currentDialogMessageGroup.currentMessage.animator.SetInteger("State", 2);
                    }
                    break;
                case DropEnum.DropDown:
                    {
                        currentDialogMessageGroup.currentMessage.animator.SetInteger("State", 3);
                    }
                    break;

                case DropEnum.DropUp:
                    {
                        currentDialogMessageGroup.currentMessage.animator.SetInteger("State", 4);
                    }
                    break;
            }
        }
    }
}
