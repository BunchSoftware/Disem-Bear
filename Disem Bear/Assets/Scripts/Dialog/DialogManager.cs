using External.Storage;
using Game.Music;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.LDialog
{
    [Serializable]
    public class DialogManager
    {
        [SerializeField] private DialogueWindow dialogueWindow;
        [SerializeField] private FileDialog fileDialog;
        public UnityEvent<Dialog> OnStartDialog;
        public UnityEvent<Dialog> OnEndDialog;
        public UnityEvent<Dialog> OnFullEndDialog;
        public UnityEvent<string> SendInputFieldText;

        private List<DialogPoint> dialogPoints = new List<DialogPoint>();
        private int currentIndexDialogPoint = 0;
        private int currentIndexDialog = 0;

        private string currentConditionSkip = "";

        private bool isCanSkipDialog = false;
        private bool isDialogLast = false;
        private bool isActiveInputField = false;
        private MonoBehaviour context;
        private SoundManager soundManager;

        private Coroutine typeLineCoroutine;

        public void Init(MonoBehaviour context, SoundManager soundManager)
        {
            this.context = context;
            this.soundManager = soundManager;

            dialogPoints = fileDialog.dialogPoints;
            dialogueWindow.Init(this, soundManager);

            if (SaveManager.filePlayer.JSONPlayer.nameUser != null)
            {
                currentIndexDialogPoint = SaveManager.filePlayer.JSONPlayer.resources.currentIndexDialogPoint;
                TypeLine(dialogPoints[currentIndexDialogPoint], SaveManager.filePlayer.JSONPlayer.resources.currentIndexDialog);
            }
        }

        public void StartDialog(int indexDialogPoint)
        {
            currentIndexDialogPoint = indexDialogPoint;
            SaveManager.filePlayer.JSONPlayer.resources.currentIndexDialogPoint = currentIndexDialogPoint;
            SaveManager.UpdatePlayerFile();
            TypeLine(dialogPoints[indexDialogPoint], currentIndexDialog);
            //if (indexDialogPoint >= currentIndexDialogPoint)
            //{
            //    currentIndexDialogPoint = indexDialogPoint;
            //    SaveManager.filePlayer.JSONPlayer.resources.currentIndexDialogPoint = currentIndexDialogPoint;
            //    TypeLine(dialogPoints[indexDialogPoint], currentIndexDialog);
            //}
        }
        public void SkipDialog()
        {
            if (isCanSkipDialog || isDialogLast && isActiveInputField == false)
            {
                Dialog dialog = null;

                if (currentIndexDialog >= 0 && currentIndexDialog <= dialogPoints[currentIndexDialogPoint].dialog.Count)
                    dialog = dialogPoints[currentIndexDialogPoint].dialog[currentIndexDialog];
                if (dialog != null
                    && dialog.skipDialog == true
                    && currentConditionSkip == dialog.conditionSkipDialog)
                {
                    StopTypeLine();

                    if (isDialogLast == true)
                    {
                        isDialogLast = false;
                        OnFullEndDialog?.Invoke(dialog);
                        ExitDrop(dialog);
                    }
                    else if (currentIndexDialog == dialogPoints[currentIndexDialogPoint].dialog.Count - 1)
                    {
                        dialogueWindow.ShowFullDialog(dialog);
                        isDialogLast = true;
                    }
                    else
                    {
                        currentIndexDialog++;
                        SaveManager.filePlayer.JSONPlayer.resources.currentIndexDialog = currentIndexDialog;
                        TypeLine(dialogPoints[currentIndexDialogPoint], currentIndexDialog);
                    }

                    currentConditionSkip = "";
                    //isCanSkipDialog = false;
                    isActiveInputField = false;
                }
            }
        }

        public void SkipDialogWithFinish()
        {
            if (isCanSkipDialog || isDialogLast && isActiveInputField == false)
            {
                Dialog dialog = null;

                if (currentIndexDialog >= 0 && currentIndexDialog <= dialogPoints[currentIndexDialogPoint].dialog.Count)
                    dialog = dialogPoints[currentIndexDialogPoint].dialog[currentIndexDialog];

                if (dialog != null && dialog.skipDialog == true)
                {
                    dialogueWindow.StopTypeLine();
                    dialogueWindow.ShowFullDialog(dialog);
                    //OnFullEndDialog?.Invoke(dialog);
                }
            }
        }

        public void RunConditionSkip(string conditionSkip)
        {
            currentConditionSkip = conditionSkip;
            SkipDialog();
        }

        private void TypeLine(DialogPoint dialogPoint, int indexDialog)
        {
            if(typeLineCoroutine != null)
                context.StopCoroutine(typeLineCoroutine);
            typeLineCoroutine = context.StartCoroutine(TypeLineIE(dialogPoint, indexDialog));
        }

        IEnumerator TypeLineIE(DialogPoint dialogPoint, int indexDialog)
        {
            currentIndexDialog = indexDialog;
            for (int i = currentIndexDialog; i < dialogPoint.dialog.Count; i++)
            {
                OnStartDialog?.Invoke(dialogPoint.dialog[i]);

                currentIndexDialog = i;
                SaveManager.filePlayer.JSONPlayer.resources.currentIndexDialog = currentIndexDialog;
                SaveManager.UpdatePlayerFile();
                if (dialogPoint.dialog[i].isActiveInputField == false)
                    isCanSkipDialog = true;

                isDialogLast = false;
                isActiveInputField = dialogPoint.dialog[i].isActiveInputField;

                dialogueWindow.StartTypeLine(dialogPoint.dialog[i]);
                EnterDrop(dialogPoint.dialog[i]);
                yield return new WaitForSeconds(dialogPoint.dialog[i].speedText * dialogPoint.dialog[i].textDialog.ToCharArray().Length);
                OnEndDialog?.Invoke(dialogPoint.dialog[i]);

                if (dialogPoint.dialog[i].stopTheEndDialog == true)
                {
                    if (currentIndexDialog == dialogPoints[currentIndexDialogPoint].dialog.Count - 1)
                        isDialogLast = true;
                    if (dialogPoint.dialog[i].skipDialog == false)
                    {
                        yield return new WaitForSeconds(dialogPoint.dialog[i].waitSecond);
                        OnFullEndDialog?.Invoke(dialogPoint.dialog[i]);
                        ExitDrop(dialogPoint.dialog[i]);
                    }
                    break;
                }
                else
                    yield return new WaitForSeconds(dialogPoint.dialog[i].waitSecond);

                OnFullEndDialog?.Invoke(dialogPoint.dialog[i]);
                ExitDrop(dialogPoint.dialog[i]);

                isCanSkipDialog = false;
                isActiveInputField = false;
            }
        }

        private void StopTypeLine()
        {;
            if (typeLineCoroutine != null)
                context.StopCoroutine(typeLineCoroutine);
            dialogueWindow.StopTypeLine();
            isCanSkipDialog = false;
            isActiveInputField = false;

            OnEndDialog?.Invoke(dialogPoints[currentIndexDialogPoint].dialog[currentIndexDialog]);
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

        public void SendInputText(string text)
        {
            if (text.Length >= 1)
            {
                SendInputFieldText?.Invoke(text);
                isActiveInputField = false;
                isCanSkipDialog = true;
                SkipDialog();
            }
        }

        public bool IsDialogRun()
        {
            return dialogueWindow.IsDialogRun();
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
