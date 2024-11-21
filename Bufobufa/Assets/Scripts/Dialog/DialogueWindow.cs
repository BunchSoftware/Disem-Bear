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
    [HideInInspector] public Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Init(DialogManager dialogManager)
    {
        skipButton.onClick.AddListener(() =>
        {
            dialogManager.SkipDialog();
        });
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
        for (int j = 0; j < dialog.textDialog.ToCharArray().Length; j++)
        {
            textDialog.text += dialog.textDialog[j];
            yield return new WaitForSeconds(dialog.speedText);
        }
    }

    public void DialogLast(Dialog dialog)
    {
        SetParametres(dialog);
        textDialog.text = dialog.textDialog;
    }

    private void SetParametres(Dialog dialog)
    {
        textDialog.fontStyle = dialog.fontStyleText;
        textDialog.fontSize = dialog.fontSizeText;
        textDialog.color = dialog.colorText;
        iconDialog.sprite = Resources.Load<Sprite>(dialog.pathToAvatar);
    }
}
