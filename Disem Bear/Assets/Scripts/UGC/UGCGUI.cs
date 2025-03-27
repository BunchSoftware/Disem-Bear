using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using External.Storage;
using SFB;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UGCGUI : MonoBehaviour
{
    [SerializeField] private GameObject prefabModPoint;
    [SerializeField] private GameObject contentModPoint;
    [SerializeField] private Button avatar;
    [SerializeField] private InputField header;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button modButton;
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject arrowUp;
    [SerializeField] private GameObject arrowDown;

    [SerializeField] private Sprite reciepAvatar;
    [SerializeField] private Sprite soundAvatar;
    [SerializeField] private Sprite textureAvatar;

    private bool isExpand= false;
    private string pathToFile;

    private UGCPoint ugcPoint;
    private List<UGCPointGUI> ugcPointsGUIs = new List<UGCPointGUI>();
    private UGCManager ugcManager;
    public void Init(UGCManager ugcManager, UGCPoint ugcPoint, string pathToFile)
    {
        this.ugcManager = ugcManager;
        this.ugcPoint = ugcPoint;
        this.pathToFile = pathToFile;

        modButton.onClick.AddListener(() =>
        {
            if (isExpand)
                Expand(false);
            else
                Expand(true);
        });

        avatar.onClick.AddListener(() => 
        {
            var extensions = new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
             };
            var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            byte[] bytes = File.ReadAllBytes(path[0]);
            Texture2D tex = new Texture2D(4, 4);
            tex.LoadImage(bytes);
            ugcPoint.avatar = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            ugcPoint.avatarBinary = bytes;
            UpdateData(ugcPoint);

            File.Delete(pathToFile);
            ugcManager.Export(ugcPoint, pathToFile);
        });


        deleteButton.onClick.AddListener(() =>
        {
            ugcManager.DeleteUGCGUI(this, pathToFile);
        });

        UGCPointGUI ugcPointSoundGUI = GameObject.Instantiate(prefabModPoint, contentModPoint.transform).GetComponent<UGCPointGUI>();
        ugcPointSoundGUI.Init(ugcPoint, soundAvatar, "Звуки", () => { ugcManager.editSoundPanel.gameObject.SetActive(true); ugcManager.editSoundPanel.UpdateData(ugcPoint, this, pathToFile); });
        ugcPointsGUIs.Add(ugcPointSoundGUI);


        UGCPointGUI ugcPointReciepeGUI = GameObject.Instantiate(prefabModPoint, contentModPoint.transform).GetComponent<UGCPointGUI>();
        ugcPointReciepeGUI.Init(ugcPoint, reciepAvatar,  "Рецепты", () => { ugcManager.editReciepePanel.gameObject.SetActive(true); ugcManager.editReciepePanel.UpdateData(ugcPoint, this, pathToFile); });
        ugcPointsGUIs.Add(ugcPointReciepeGUI);

        UGCPointGUI ugcPointTexturesGUI = GameObject.Instantiate(prefabModPoint, contentModPoint.transform).GetComponent<UGCPointGUI>();
        ugcPointTexturesGUI.Init(ugcPoint, textureAvatar, "Текстуры", () => { ugcManager.editTexturePanel.gameObject.SetActive(true); ugcManager.editTexturePanel.UpdateData(ugcPoint, this, pathToFile);  });
        ugcPointsGUIs.Add(ugcPointTexturesGUI);

        arrowDown.gameObject.SetActive(false);
        arrowUp.gameObject.SetActive(true);

        header.onValueChanged.AddListener((value) =>
        {
            if (File.Exists(Path.GetFullPath(this.pathToFile)))
            {
                File.Move(Path.GetFullPath(this.pathToFile), Path.GetFullPath(Path.Combine(SaveManager.pathToDirectoryMod, $"{value}.modbuf")));
                this.pathToFile = Path.GetFullPath(Path.Combine(SaveManager.pathToDirectoryMod, $"{value}.modbuf"));
                ugcPoint.nameUGCPoint = value;

            }
        });
        UpdateData(ugcPoint);
    }

    public void UpdateData(UGCPoint ugcPoint)
    {
        this.ugcPoint = ugcPoint;
        if(ugcPoint.avatarBinary != null && ugcPoint.avatarBinary.Length > 0)
        {
            try
            {
                byte[] bytes = ugcPoint.avatarBinary;
                Texture2D tex = new Texture2D(4, 4);
                tex.LoadImage(bytes);
                ugcPoint.avatar = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                avatar.GetComponent<Image>().sprite = ugcPoint.avatar;
            }
            catch
            {
                ugcPoint.avatar = null;
            }
        }
        if(ugcPoint.avatar != null)
            avatar.GetComponent<Image>().sprite = ugcPoint.avatar;
        header.text = ugcPoint.nameUGCPoint;
    }
    public void Expand(bool isExpand)
    {
        this.isExpand= isExpand;
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


    public UGCPoint GetUGCPoint()
    {
        return ugcPoint;
    }
}
