using External.Storage;
using Game.Music;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.LDialog
{
    [Serializable]
    public class DialogManager
    {
        [SerializeField] private DialogueWindow dialogueWindow;
        [SerializeField] private DialogDatabase fileDialog;

        public UnityEvent<Dialog> OnStartDialog;
        public UnityEvent<Dialog> OnEndDialog;
        public UnityEvent<Dialog> OnFullEndDialog;
        public UnityEvent<string> OnSendInputFieldText;

        private int currentIndexDialogPoint = 0;
        private int currentIndexDialog = 0;
        private bool isDialogOn = false;

        private bool isCanSkipReplica = false;
        private bool isDialogLast = false;
        private bool isActiveInputField = false;
        private MonoBehaviour context;
        private SoundManager soundManager;

        private Coroutine typeLineCoroutine;

        public void Init(MonoBehaviour context, SoundManager soundManager)
        {
            this.context = context;
            this.soundManager = soundManager;

            dialogueWindow.Init(this, soundManager);
            dialogueWindow.OnSendInputFieldText.AddListener((text) =>
            {
                OnSendInputFieldText?.Invoke(text);
                isActiveInputField = false;
                isCanSkipReplica = true;
                SkipReplica();
            });

            if (SaveManager.playerDatabase.JSONPlayer.nameUser != null)
            {
                currentIndexDialogPoint = SaveManager.playerDatabase.JSONPlayer.resources.currentIndexDialogPoint;
                TypeLine(fileDialog.dialogPoints[currentIndexDialogPoint], SaveManager.playerDatabase.JSONPlayer.resources.currentIndexDialog);
            }


            Debug.Log("DialogManager: Успешно иницилизирован");
        }

        public void StartDialog(int indexDialogPoint)
        {
            currentIndexDialogPoint = indexDialogPoint;
            SaveManager.playerDatabase.JSONPlayer.resources.currentIndexDialogPoint = currentIndexDialogPoint;
            SaveManager.UpdatePlayerDatabase();
            TypeLine(fileDialog.dialogPoints[indexDialogPoint], currentIndexDialog);
        }

        public void SkipReplica()
        {
            if (isCanSkipReplica || (isDialogLast && isActiveInputField == false))
            {
                Dialog dialog = null;

                if (currentIndexDialog >= 0 && currentIndexDialog <= fileDialog.dialogPoints[currentIndexDialogPoint].dialog.Count)
                    dialog = fileDialog.dialogPoints[currentIndexDialogPoint].dialog[currentIndexDialog];
                if (dialog != null)
                {
                    StopTypeLine();
                    if (currentIndexDialog == fileDialog.dialogPoints[currentIndexDialogPoint].dialog.Count - 1)
                        isDialogLast = true;
                    if (isDialogLast == true)
                    {
                        isDialogLast = false;
                        OnFullEndDialog?.Invoke(dialog);
                        ExitDrop(dialog);
                        isDialogOn = false;
                    }
                    else if (currentIndexDialog == fileDialog.dialogPoints[currentIndexDialogPoint].dialog.Count - 1)
                    {
                        dialogueWindow.ShowFullDialog(dialog);
                        isDialogLast = true;
                    }
                    else
                    {
                        currentIndexDialog++;
                        SaveManager.playerDatabase.JSONPlayer.resources.currentIndexDialog = currentIndexDialog;
                        TypeLine(fileDialog.dialogPoints[currentIndexDialogPoint], currentIndexDialog);
                    }

                    isActiveInputField = false;
                }
            }
        }

        public void SkipReplicaWithFinish()
        {
            if (isCanSkipReplica || (isDialogLast && isActiveInputField == false))
            {
                Dialog dialog = null;

                if (currentIndexDialog >= 0 && currentIndexDialog <= fileDialog.dialogPoints[currentIndexDialogPoint].dialog.Count)
                    dialog = fileDialog.dialogPoints[currentIndexDialogPoint].dialog[currentIndexDialog];

                if (dialog != null)
                {
                    dialogueWindow.StopTypeLine();
                    dialogueWindow.ShowFullDialog(dialog);
                }
            }
        }

        private void TypeLine(DialogPoint dialogPoint, int indexDialog)
        {
            if (typeLineCoroutine != null)
                context.StopCoroutine(typeLineCoroutine);
            typeLineCoroutine = context.StartCoroutine(TypeLineIE(dialogPoint, indexDialog));
        }

        private IEnumerator TypeLineIE(DialogPoint dialogPoint, int indexDialog)
        {
            for (int i = indexDialog; i < dialogPoint.dialog.Count; i++)
            {
                OnStartDialog?.Invoke(dialogPoint.dialog[i]);

                currentIndexDialog = i;
                SaveManager.playerDatabase.JSONPlayer.resources.currentIndexDialog = currentIndexDialog;
                SaveManager.UpdatePlayerDatabase();

                isCanSkipReplica = !dialogPoint.dialog[i].isActiveInputField;
                isDialogLast = false;
                isActiveInputField = dialogPoint.dialog[i].isActiveInputField;

                EnterDrop(dialogPoint.dialog[i]);
                isDialogOn = true;
                dialogueWindow.StartTypeLine(dialogPoint.dialog[i]);

                yield return new WaitForSeconds(dialogPoint.dialog[i].speedText * dialogPoint.dialog[i].textDialog.Length);
                OnEndDialog?.Invoke(dialogPoint.dialog[i]);

                if (dialogPoint.dialog[i].stopTheEndDialog == true)
                {
                    if (currentIndexDialog == fileDialog.dialogPoints[currentIndexDialogPoint].dialog.Count - 1)
                        isDialogLast = true;
                    if (dialogPoint.dialog[i].skipDialogButton == false)
                        yield return new WaitForSeconds(dialogPoint.dialog[i].waitSecond);

                    //OnFullEndDialog?.Invoke(dialogPoint.dialog[i]);
                    break;
                }
                else
                    yield return new WaitForSeconds(dialogPoint.dialog[i].waitSecond);

                //OnFullEndDialog?.Invoke(dialogPoint.dialog[i]);
                //ExitDrop(dialogPoint.dialog[i]);
            }
        }

        private void StopTypeLine()
        {
            if (typeLineCoroutine != null)
                context.StopCoroutine(typeLineCoroutine);

            dialogueWindow.StopTypeLine();
            isCanSkipReplica = false;
            isActiveInputField = false;
            //isDialogLast = false;

            OnEndDialog?.Invoke(fileDialog.dialogPoints[currentIndexDialogPoint].dialog[currentIndexDialog]);
        }

        private void EnterDrop(Dialog dialog)
        {
            switch (dialog.enterDrop)
            {
                case DropEnum.DropRight:
                    {
                        dialogueWindow.animator.SetInteger("State", 1);
                    }
                    break;
                case DropEnum.DropLeft:
                    {
                        dialogueWindow.animator.SetInteger("State", 2);
                    }
                    break;
                case DropEnum.DropDown:
                    {
                        dialogueWindow.animator.SetInteger("State", 3);
                    }
                    break;

                case DropEnum.DropUp:
                    {
                        dialogueWindow.animator.SetInteger("State", 4);
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
                        dialogueWindow.animator.SetInteger("State", 1);
                    }
                    break;
                case DropEnum.DropLeft:
                    {
                        dialogueWindow.animator.SetInteger("State", 2);
                    }
                    break;
                case DropEnum.DropDown:
                    {
                        dialogueWindow.animator.SetInteger("State", 3);
                    }
                    break;

                case DropEnum.DropUp:
                    {
                        dialogueWindow.animator.SetInteger("State", 4);
                    }
                    break;
            }
        }

        public bool IsDialogOn()
        {
            return isDialogOn;
        }


        public int GetCurrentIndexDialog()
        {
            return currentIndexDialog;
        }

        public int GetCurrentIndexDialogPoint()
        {
            return currentIndexDialogPoint;
        }
    }
}
