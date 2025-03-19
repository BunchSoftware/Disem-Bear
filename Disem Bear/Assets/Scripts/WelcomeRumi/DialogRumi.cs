using System.Collections;
using System.Collections.Generic;
using External.API;
using External.Storage;
using Game.LDialog;
using Game.Music;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogRumi : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int nextScene = 0;
    [SerializeField] private APIManager apiManager;
    [SerializeField] private FilePlayer filePlayer;
    [SerializeField] private FileShop fileShop;
    [SerializeField] private FilePlayer defaultFilePlayer;
    [SerializeField] private FileShop defaultFileShop;
    [SerializeField] private GameObject fade;
    [SerializeField] private GameObject fadeEnd;
    [SerializeField] private TMP_InputField inputField;
    private TextMeshProUGUI textDialog;
    private bool isDialogRun = false;
    private int numDialog = 0;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDialogRun && numDialog == 0)
        {
            StopAllCoroutines();
            textDialog.text = "Привет пушистик. Сегодня я научу тебя магии. Ах да, думаю стоит начать со знакомства :)";
            numDialog = 1;
            isDialogRun = false;
        }
        else if (numDialog == 1 && isDialogRun == false)
        {
            inputField.gameObject.SetActive(true);
            Dialog firstdialog = new();
            firstdialog.textDialog = "Я руми (`w`)";
            firstdialog.speedText = 0.05f;
            StartCoroutine(TypeLineIE(firstdialog));
        }
        else if (numDialog == 1 && isDialogRun)
        {
            StopAllCoroutines();
            textDialog.text = "Я руми (`w`)";
            numDialog = 2;
            isDialogRun = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputField.text != "")
            {
                fadeEnd.SetActive(true);
                fadeEnd.GetComponent<Animator>().Play("FadeEnd");
                StartCoroutine(cor());
            }
        }
    }

    IEnumerator cor()
    {
        yield return new WaitForSeconds(1f);
        SaveManager.RegistrationPlayer(inputField.text);

        SceneManager.LoadScene(nextScene);
    }

    private void Start()
    {
        fade.SetActive(true);
        StartCoroutine(RemoveFade());
        textDialog = GetComponent<TextMeshProUGUI>();
        SaveManager.Init(apiManager, filePlayer, fileShop, defaultFilePlayer, defaultFileShop);
        Dialog firstdialog = new();
        firstdialog.textDialog = "Привет пушистик. Сегодня я научу тебя магии. Ах да, думаю стоит начать со знакомства :)";
        firstdialog.speedText = 0.05f;
        StartCoroutine(TypeLineIE(firstdialog));
    }

    IEnumerator RemoveFade()
    {
        yield return new WaitForSeconds(0.5f);
        fade.SetActive(false);
    }

    IEnumerator TypeLineIE(Dialog dialog)
    {
        textDialog.text = "";

        isDialogRun = true;

        for (int j = 0; j < dialog.textDialog.ToCharArray().Length; j++)
        {
            yield return new WaitForSeconds(dialog.speedText);
            //if (dialog.soundText != null)
            //    soundManager.OnPlayOneShot(dialog.soundText);

            textDialog.text += dialog.textDialog[j];
        }

        isDialogRun = false;
        numDialog++;
    }
}
