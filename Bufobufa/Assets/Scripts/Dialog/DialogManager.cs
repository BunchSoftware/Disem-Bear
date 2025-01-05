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

[Serializable]
public class DialogManager
{ 
    [SerializeField] private DialogueWindow dialogueWindow;
    [SerializeField] private FileDialog fileDialog;
    public UnityEvent<Dialog> OnStartDialog;
    public UnityEvent<Dialog> OnEndDialog;
    public UnityEvent<string> SendInputFieldText;

    private List<DialogPoint> dialogPoints = new List<DialogPoint>();
    private int currentIndexDialogPoint = 0;
    private int currentIndexDialog = 0;

    private string currentConditionSkip = "";

    private bool isCanSkipDialog = false;
    private bool isDialogLast = false;
    private bool isActiveInputField = false;

    private SaveManager saveManager;
    private MonoBehaviour context;

    public void Init(SaveManager saveManager, MonoBehaviour context)
    {
        this.context = context;
        this.saveManager = saveManager;

        dialogPoints = fileDialog.dialogPoints;
        dialogueWindow.Init(this);

        if (saveManager.filePlayer.JSONPlayer.nameUser != null)
        {
            currentIndexDialogPoint = saveManager.filePlayer.JSONPlayer.resources.currentIndexDialogPoint;
            int count = saveManager.filePlayer.JSONPlayer.resources.currentIndexDialog;
            for (int i = 0; i < count; i++)
            {
                OnStartDialog?.Invoke(dialogPoints[currentIndexDialogPoint].dialog[i]);
            }
            TypeLine(dialogPoints[currentIndexDialogPoint], saveManager.filePlayer.JSONPlayer.resources.currentIndexDialog);
        }
    }

    public void StartDialog(int indexDialogPoint)
    {
        if (indexDialogPoint >= currentIndexDialogPoint)
        {
            currentIndexDialogPoint = indexDialogPoint;
            saveManager.filePlayer.JSONPlayer.resources.currentIndexDialogPoint = currentIndexDialogPoint;
            saveManager.UpdatePlayerFile();
            TypeLine(dialogPoints[indexDialogPoint], currentIndexDialog);
        }
    }
    public void SkipDialog()
    {
        if(isCanSkipDialog || isDialogLast && isActiveInputField == false)
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
                    ExitDrop(dialog);
                }
                else if (currentIndexDialog == dialogPoints[currentIndexDialogPoint].dialog.Count - 1)
                {
                    dialogueWindow.DialogLast(dialog);
                    isDialogLast = true;
                }
                else
                {
                    currentIndexDialog++;
                    saveManager.filePlayer.JSONPlayer.resources.currentIndexDialog = currentIndexDialog;
                    saveManager.UpdatePlayerFile();
                    TypeLine(dialogPoints[currentIndexDialogPoint], currentIndexDialog);
                }

                currentConditionSkip = "";
            }
        }
    }

    public void RunConditionSkip(string conditionSkip)
    {
        currentConditionSkip = conditionSkip;
        SkipDialog();
    }

    public void TypeLine(DialogPoint dialogPoint, int indexDialog)
    {
        context.StopAllCoroutines();
        context.StartCoroutine(TypeLineIE(dialogPoint, indexDialog));
    }

    IEnumerator TypeLineIE(DialogPoint dialogPoint, int indexDialog)
    {
        currentIndexDialog = indexDialog;
        for (int i = currentIndexDialog; i < dialogPoint.dialog.Count; i++)
        {
            OnStartDialog?.Invoke(dialogPoint.dialog[i]);

            currentIndexDialog = i;
            saveManager.filePlayer.JSONPlayer.resources.currentIndexDialog = currentIndexDialog;
            saveManager.UpdatePlayerFile();

            if (dialogPoint.dialog[i].isActiveInputField == false)
                isCanSkipDialog = true;

            isDialogLast = false;
            isActiveInputField = dialogPoint.dialog[i].isActiveInputField;

            EnterDrop(dialogPoint.dialog[i]);
            dialogueWindow.StartTypeLine(dialogPoint.dialog[i]);
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueWindow.gameObject.GetComponent<RectTransform>());
            yield return new WaitForSeconds(dialogPoint.dialog[i].speedText * dialogPoint.dialog[i].textDialog.Length);

            OnEndDialog?.Invoke(dialogPoint.dialog[i]);

            if (dialogPoint.dialog[i].stopTheEndDialog == true)
            {
                if (currentIndexDialog == dialogPoints[currentIndexDialogPoint].dialog.Count - 1)
                    isDialogLast = true;
                if(dialogPoint.dialog[i].skipDialog == false)
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
            isActiveInputField = false;
        }
    }

    private void StopTypeLine() 
    {
        context.StopAllCoroutines();
        dialogueWindow.StopTypeLine();
        isCanSkipDialog = false;
        isActiveInputField = false;
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
        if(text.Length >= 1)
        {
            SendInputFieldText?.Invoke(text);
            isActiveInputField = false;
            isCanSkipDialog = true;
            SkipDialog();
        }
    }
}
