using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class EditSoundPanel : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject prefabEditSoundGUI;
    [SerializeField] private Button addNewSound;
    [SerializeField] private Button deleteAllSound;

    private List<EditSoundGUI> editSoundGUIs = new List<EditSoundGUI>();
    private string pathToFile;
    private UGCPoint ugcPoint;
    private UGCManager ugcManager;
    private UGCGUI ugcGUI;
    public void Init(UGCManager ugcManager)
    {
        this.ugcManager = ugcManager;

        addNewSound.onClick.AddListener(() => 
        {
            AddEditSoundGUI(new UGCSound(), this.pathToFile);
        });
        deleteAllSound.onClick.AddListener(() => 
        {
            for (int i = 0; i < editSoundGUIs.Count; i++)
            {
                Destroy(editSoundGUIs[i].gameObject);
            }
            editSoundGUIs.Clear();

            File.Delete(pathToFile);
            ugcPoint.ugcSounds.Clear();
            ugcManager.Export(ugcPoint, pathToFile);
        });
    }
    public void UpdateData(UGCPoint ugcPoint,UGCGUI ugcGUI, string pathToFile)
    {
        this.pathToFile = pathToFile;
        this.ugcPoint = ugcPoint;
        this.ugcGUI = ugcGUI;
        for (int i = 0; i < editSoundGUIs.Count; i++)
        {
            Destroy(editSoundGUIs[i].gameObject);
        }
        editSoundGUIs.Clear();
        for (int i = 0; i < ugcPoint.ugcSounds.Count; i++)
        {
            EditSoundGUI editSoundGUI = Instantiate(prefabEditSoundGUI, content.transform).GetComponent<EditSoundGUI>();
            editSoundGUI.Init(this, ugcPoint.ugcSounds[i], pathToFile);
            editSoundGUIs.Add(editSoundGUI);
            LayoutRebuilder.ForceRebuildLayoutImmediate(editSoundGUI.GetComponent<RectTransform>());
        }

        ugcGUI.UpdateData(ugcPoint);

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    public void DeleteSoundGUI(EditSoundGUI editSoundGUI)
    {
        File.Delete(pathToFile);
        ugcPoint.ugcSounds.Remove(editSoundGUI.GetUGCSound());
        ugcManager.Export(ugcPoint, pathToFile);

        Destroy(editSoundGUI.gameObject);
        editSoundGUIs.Remove(editSoundGUI);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }
    private void AddEditSoundGUI(UGCSound ugcSound, string pathToFile)
    {
        EditSoundGUI editSoundGUI = Instantiate(prefabEditSoundGUI, content.transform).GetComponent<EditSoundGUI>();
        editSoundGUI.Init(this, ugcSound, pathToFile);

        LayoutRebuilder.ForceRebuildLayoutImmediate(editSoundGUI.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        File.Delete(pathToFile);
        ugcPoint.ugcSounds.Add(ugcSound);
        ugcManager.Export(ugcPoint, pathToFile);

        editSoundGUIs.Add(editSoundGUI);
    }
}
