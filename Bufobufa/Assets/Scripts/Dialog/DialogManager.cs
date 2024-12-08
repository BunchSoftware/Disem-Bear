using Newtonsoft.Json;
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

public class DialogManager : MonoBehaviour
{
    [SerializeField] private DialogueWindow dialogueWindow;
    [SerializeField] private FileDialog fileDialog;
    public UnityEvent<Dialog> EndDialog;
    public UnityEvent<string> SendInputFieldText;

    private List<DialogPoint> dialogPoints = new List<DialogPoint>();
    private int currentIndexDialogPoint = 0;
    private int currentIndexDialog = 0;

    private string currentConditionSkip = "";

    private bool isCanSkipDialog = false;
    private bool isDialogLast = false;
    private bool isActiveInputField = false;

    private void Start()
    {
        dialogPoints = fileDialog.dialogPoints;
        dialogueWindow.Init(this);
    }

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    public void StartDialog(int indexDialogPoint)
    {
        currentIndexDialogPoint = indexDialogPoint;
        TypeLine(dialogPoints[indexDialogPoint], 0);
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
        StopAllCoroutines();
        StartCoroutine(TypeLineIE(dialogPoint, indexDialog));
    }

    IEnumerator TypeLineIE(DialogPoint dialogPoint, int indexDialog)
    {
        currentIndexDialog = indexDialog;
        for (int i = currentIndexDialog; i < dialogPoint.dialog.Count; i++)
        {
            currentIndexDialog = i;
            if (dialogPoint.dialog[i].isActiveInputField == false)
                isCanSkipDialog = true;

            isDialogLast = false;
            isActiveInputField = dialogPoint.dialog[i].isActiveInputField;

            EnterDrop(dialogPoint.dialog[i]);
            dialogueWindow.StartTypeLine(dialogPoint.dialog[i]);
            yield return new WaitForSeconds(dialogPoint.dialog[i].speedText * dialogPoint.dialog[i].textDialog.Length);

            EndDialog?.Invoke(dialogPoint.dialog[i]);

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
        StopAllCoroutines();
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
            SkipDialog();
        }
    }
}
