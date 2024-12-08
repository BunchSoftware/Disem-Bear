using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private Text textDialog;
    [SerializeField] private Image iconDialog;
    [SerializeField] private Button skipButton;
    [SerializeField] private DialogInputField dialogInputField;
    [SerializeField] private Image imageClickButtonForSkip;
    [HideInInspector] public Animator animator;

    private Font standartFont;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Init(DialogManager dialogManager)
    {
        standartFont = textDialog.font;
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(() =>
        {
            dialogManager.SkipDialog();
        });

        dialogInputField.Init(dialogManager);
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
        textDialog.text = "";
        SetParametres(dialog);
        dialogInputField.SetParametres(dialog);
        for (int j = 0; j < dialog.textDialog.ToCharArray().Length; j++)
        {
            textDialog.text += dialog.textDialog[j];
            yield return new WaitForSeconds(dialog.speedText);
        }
    }

    public void DialogLast(Dialog dialog)
    {
        SetParametres(dialog);
        dialogInputField.SetParametres(dialog);
        textDialog.text = dialog.textDialog;
    }

    private void SetParametres(Dialog dialog)
    {
        if (dialog.fontText != null)
            textDialog.font = dialog.fontText;
        else
            textDialog.font = standartFont;
        textDialog.fontStyle = dialog.fontStyleText;
        textDialog.fontSize = dialog.fontSizeText;
        textDialog.color = dialog.colorText;
        iconDialog.sprite = dialog.avatar;
        iconDialog.preserveAspect = true;

        if (dialog.imageClickButtonForSkip != null)
        {
            imageClickButtonForSkip.sprite = dialog.imageClickButtonForSkip;
            imageClickButtonForSkip.preserveAspect = true;
            imageClickButtonForSkip.gameObject.SetActive(true);
        }
        else
            imageClickButtonForSkip.gameObject.SetActive(false);

        dialogInputField.gameObject.SetActive(dialog.isActiveInputField);
    }
}
