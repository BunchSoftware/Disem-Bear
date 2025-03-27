using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EditTexturePanel : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject prefabEditTextureGUI;
    [SerializeField] private Button addNewTexture;
    [SerializeField] private Button deleteAllTexture;

    private List<EditTextureGUI> editTextureGUIs = new List<EditTextureGUI>();
    private string pathToFile;
    private UGCPoint ugcPoint;
    private UGCManager ugcManager;
    private UGCGUI ugcGUI;
    public void Init(UGCManager ugcManager)
    {
        this.ugcManager = ugcManager;

        addNewTexture.onClick.AddListener(() =>
        {
            AddEditTextureGUI(new UGCTexture(), this.pathToFile);
        });
        deleteAllTexture.onClick.AddListener(() =>
        {
            for (int i = 0; i < editTextureGUIs.Count; i++)
            {
                Destroy(editTextureGUIs[i].gameObject);
            }
            editTextureGUIs.Clear();

            File.Delete(pathToFile);
            ugcPoint.ugcTextures.Clear();
            ugcManager.Export(ugcPoint, pathToFile);
        });
    }
    public void UpdateData(UGCPoint ugcPoint, UGCGUI ugcGUI, string pathToFile)
    {
        this.pathToFile = pathToFile;
        this.ugcPoint = ugcPoint;
        this.ugcGUI = ugcGUI;
        for (int i = 0; i < editTextureGUIs.Count; i++)
        {
            Destroy(editTextureGUIs[i].gameObject);
        }
        editTextureGUIs.Clear();
        for (int i = 0; i < ugcPoint.ugcTextures.Count; i++)
        {
            EditTextureGUI editTextureGUI = Instantiate(prefabEditTextureGUI, content.transform).GetComponent<EditTextureGUI>();
            editTextureGUI.Init(this, ugcPoint.ugcTextures[i], pathToFile);
            editTextureGUIs.Add(editTextureGUI);
            LayoutRebuilder.ForceRebuildLayoutImmediate(editTextureGUI.GetComponent<RectTransform>());
        }

        ugcGUI.UpdateData(ugcPoint);

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    public void DeleteTextureGUI(EditTextureGUI editTextureGUI)
    {
        File.Delete(pathToFile);
        ugcPoint.ugcTextures.Remove(editTextureGUI.GetUGCTexture());
        ugcManager.Export(ugcPoint, pathToFile);

        Destroy(editTextureGUI.gameObject);
        editTextureGUIs.Remove(editTextureGUI);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }
    private void AddEditTextureGUI(UGCTexture ugcTexture, string pathToFile)
    {
        EditTextureGUI editTextureGUI = Instantiate(prefabEditTextureGUI, content.transform).GetComponent<EditTextureGUI>();
        editTextureGUI.Init(this, ugcTexture, pathToFile);

        LayoutRebuilder.ForceRebuildLayoutImmediate(editTextureGUI.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        File.Delete(pathToFile);
        ugcPoint.ugcTextures.Add(ugcTexture);
        ugcManager.Export(ugcPoint, pathToFile);

        editTextureGUIs.Add(editTextureGUI);
    }
}
