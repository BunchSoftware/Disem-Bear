using External.API;
using External.Storage;
using Game.LDialog;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DialogRumi : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int nextScene = 0;
    [SerializeField] private APIManager apiManager;
    [SerializeField] private PlayerDatabase filePlayer;
    [SerializeField] private ShopDatabase fileShop;
    [SerializeField] private UGCDatabase fileUGC;
    [SerializeField] private PlayerDatabase defaultFilePlayer;
    [SerializeField] private ShopDatabase defaultFileShop;
    [SerializeField] private UGCDatabase defaultFileUGC;
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
            Dialog firstdialog = new();
            firstdialog.textDialog = "Я Руми ( `w` )\r\n\r\nА я";
            firstdialog.speedText = 0.05f;
            StartCoroutine(TypeLineIE(firstdialog));
        }
        else if (numDialog == 1 && isDialogRun)
        {
            inputField.gameObject.SetActive(true);
            StopAllCoroutines();
            textDialog.text = "Я Руми ( `w` )\r\n\r\nА я";
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

    private IEnumerator cor()
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
        SaveManager.Init(apiManager, filePlayer, fileShop, fileUGC, defaultFilePlayer, defaultFileShop, defaultFileUGC);
        Dialog firstdialog = new();
        firstdialog.textDialog = "Привет пушистик. Сегодня я научу тебя магии. Ах да, думаю стоит начать со знакомства :)";
        firstdialog.speedText = 0.05f;
        StartCoroutine(TypeLineIE(firstdialog));
    }

    private IEnumerator RemoveFade()
    {
        yield return new WaitForSeconds(0.5f);
        fade.SetActive(false);
    }

    private IEnumerator TypeLineIE(Dialog dialog)
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
        if (numDialog == 1)
        {
            inputField.gameObject.SetActive(true);
        }
        numDialog++;
    }
}
