using System.Collections;
using System.Collections.Generic;
using System.IO;
using External.Storage;
using UnityEngine;
using UnityEngine.UI;

public class EditSoundGUI : MonoBehaviour
{
    [SerializeField] private InputField header;
    [SerializeField] private Button editSoundGUIButton;
    [SerializeField] private Button deleteSound;
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject arrowUp;
    [SerializeField] private GameObject arrowDown;

    private bool isExpand = false;
    private string pathToFile;

    private UGCSound ugcSound;
    private EditSoundPanel editSoundPanel;
    public void Init(EditSoundPanel editSoundPanel, UGCSound ugcSound, string pathToFile)
    {
        this.editSoundPanel = editSoundPanel;
        this.ugcSound = ugcSound;
        this.pathToFile = pathToFile;

        editSoundGUIButton.onClick.AddListener(() =>
        {
            if (isExpand)
                Expand(false);
            else
                Expand(true);
        });

        deleteSound.onClick.AddListener(() => { editSoundPanel.DeleteSoundGUI(this); });

        //avatar.onClick.AddListener(() =>
        //{
        //    var extensions = new[] {
        //        new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
        //     };
        //    var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        //    byte[] bytes = File.ReadAllBytes(path[0]);
        //    Texture2D tex = new Texture2D(4, 4);
        //    tex.LoadImage(bytes);
        //    ugcPoint.avatar = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        //    ugcPoint.avatarBinary = bytes;
        //    UpdateData(ugcPoint);

        //    File.Delete(pathToFile);
        //    ugcManager.Export(ugcPoint, pathToFile);
        //});

        
        arrowDown.gameObject.SetActive(false);
        arrowUp.gameObject.SetActive(true);

        //header.onValueChanged.AddListener((value) =>
        //{
        //    if (value != null)
        //    {
        //        File.Move(Path.GetFullPath(this.pathToFile), Path.GetFullPath(Path.Combine(SaveManager.pathToDirectoryMod, $"{value}.modbuf")));
        //        this.pathToFile = Path.GetFullPath(Path.Combine(SaveManager.pathToDirectoryMod, $"{value}.modbuf"));
        //        this.ugcSound.nameMasterSound = value;
        //    }
        //});

        UpdateData(this.ugcSound);
    }

    public void UpdateData(UGCSound ugcSound)
    {
        this.ugcSound = ugcSound;
        
        header.text = ugcSound.nameMasterSound;
    }

    public UGCSound GetUGCSound()
    {
        return ugcSound;
    }
    public void Expand(bool isExpand)
    {
        this.isExpand = isExpand;
        arrowDown.gameObject.SetActive(isExpand);
        arrowUp.gameObject.SetActive(!isExpand);
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
        if (description != null)
        {
            description.gameObject.SetActive(isExpand);
        }
        else
            throw new System.Exception("Ошибка ! Добавьте обьект Description");
    }
}
