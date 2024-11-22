using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DialogSide
{
    Right = 0, 
    Left = 1,
}

public class DialogMessageGroup : MonoBehaviour
{
    [SerializeField] private DialogMessage messageRight;
    [SerializeField] private DialogMessage messageLeft;

    private DialogSide currentDialogSide;
    [HideInInspector] public DialogMessage currentMessage;

    public void Init(DialogChat dialogChat, DialogSide dialogSide)
    {
        switch (dialogSide)
        {
            case DialogSide.Right:
                {
                    currentMessage = messageRight;
                    messageLeft.gameObject.SetActive(false);
                }
                break;
            case DialogSide.Left:
                {
                    currentMessage = messageLeft;
                    messageRight.gameObject.SetActive(false);
                }
                break;
        }
        currentMessage.Init();
    }

    public void StartTypeLine(Dialog dialog)
    {
        StopAllCoroutines();
        StartCoroutine(TypeLineIE(dialog));
    }

    public void StopTypeLine()
    {
        StopAllCoroutines();
    }

    IEnumerator TypeLineIE(Dialog dialog)
    {
        currentMessage.textMessage.text = "";
        SetParametres(dialog);
        for (int j = 0; j < dialog.textDialog.ToCharArray().Length; j++)
        {
            currentMessage.textMessage.text += dialog.textDialog[j];
            yield return new WaitForSeconds(dialog.speedText);
        }


        float y = currentMessage.textMessage.transform.GetComponent<RectTransform>().sizeDelta.y +
            (Math.Abs(currentMessage.iconMessage.transform.GetComponent<RectTransform>().position.y
            - Math.Abs(currentMessage.textMessage.transform.parent.GetComponent<RectTransform>().position.y)));
        transform.GetComponent<RectTransform>().sizeDelta
            = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, y);

        print(y);
    }

    public void DialogLast(Dialog dialog)
    {
        SetParametres(dialog);
        currentMessage.textMessage.text = dialog.textDialog;
    }

    private void SetParametres(Dialog dialog)
    {
        currentMessage.textMessage.fontStyle = dialog.fontStyleText;
        currentMessage.textMessage.fontSize = dialog.fontSizeText;
        currentMessage.textMessage.color = dialog.colorText;
        currentMessage.iconMessage.sprite = dialog.avatar;
    }
}
