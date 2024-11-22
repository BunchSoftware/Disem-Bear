using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private DialogueWindow dialogueWindow;
    [SerializeField] private TextAsset JsonFile;
    public UnityEvent<Dialog> EndDialog;

    private List<DialogPoint> dialogPoints = new List<DialogPoint>();
    private int currentIndexDialogPoint = 0;
    private int currentIndexDialog = 0;

    private bool isCanSkipDialog = false;
    private bool isDialogLast = false;

    private void Start()
    {
        string path = AssetDatabase.GetAssetPath(JsonFile);
        dialogPoints = JsonConvert.DeserializeObject<List<DialogPoint>>(File.ReadAllText(path));

        for (int i = 0; i < dialogPoints.Count; i++)
        {
            for (int j = 0; j < dialogPoints[i].dialog.Count; j++)
            {
                ColorUtility.TryParseHtmlString(dialogPoints[i].dialog[j].jsonHTMLColorRGBA, out dialogPoints[i].dialog[j].colorText);
                dialogPoints[i].dialog[j].avatar = AssetDatabase.LoadAssetAtPath<Sprite>(dialogPoints[i].dialog[j].pathToAvatar);
                dialogPoints[i].dialog[j].fontText = AssetDatabase.LoadAssetAtPath<Font>(dialogPoints[i].dialog[j].pathToFont);
            }
        }

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
        if(isCanSkipDialog || isDialogLast)
        {
            Dialog dialog = null;

            if (currentIndexDialog >= 0 && currentIndexDialog <= dialogPoints[currentIndexDialogPoint].dialog.Count)
                dialog = dialogPoints[currentIndexDialogPoint].dialog[currentIndexDialog];

            if (dialog != null && dialog.skipDialog == true)
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
            }
        }
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
            isCanSkipDialog = true;
            isDialogLast = false;

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
        }
    }

    private void StopTypeLine() 
    {
        StopAllCoroutines();
        dialogueWindow.StopTypeLine();
        isCanSkipDialog = false;
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
}
