using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using External.Storage;
using UI.PlaneTablet.Exercise;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UGCManager 
{
    [SerializeField] private GameObject prefabMode;
    [SerializeField] private GameObject content;
    public EditReciepePanel editReciepePanel;
    public EditSoundPanel editSoundPanel;
    public EditTexturePanel editTexturePanel;
    [SerializeField] private Button updateButton;
    [SerializeField] private Button createButton;
    [SerializeField] private UGCDatabase ugcDatabase;
    
    private List<UGCGUI> ugcGUIs = new List<UGCGUI>();   
    public void Init()
    {
        editSoundPanel.Init(this);
        editTexturePanel.Init(this);
        editReciepePanel.Init(this);

        updateButton.onClick.AddListener(() => 
        {
            UpdateUGC();
        });

        createButton.onClick.AddListener(() => 
        {
            UGCPoint ugcPoint = new UGCPoint();
            ugcPoint.nameUGCPoint = $"Default{ugcGUIs.Count}";
            AddUGCGUI(ugcPoint, SaveManager.pathToDirectoryModExport + $"Default{ugcGUIs.Count}.modbuf");
        });

        //Export(new UGCPoint(), SaveManager.pathToDirectoryModExport + $"Default.modbuf");
        UpdateUGC();
    }

    private void UpdateUGC()
    {
        for (int i = 0; i < ugcGUIs.Count; i++)
        {
            GameObject.Destroy(ugcGUIs[i].gameObject);
        }
        ugcGUIs.Clear();
        List<string> nameFiles = new List<string>();
        if (Directory.Exists(Path.GetFullPath(SaveManager.pathToDirectoryMod)))
        {
            nameFiles = Directory.GetFiles(Path.GetFullPath(SaveManager.pathToDirectoryMod)).ToList();
        }
        else
        {
            Directory.CreateDirectory(Path.GetFullPath(SaveManager.pathToDirectoryMod));
        }
        for (int i = 0; i < nameFiles.Count; i++)
        {
            if (Path.GetExtension(nameFiles[i]) == ".modbuf")
            {
                UGCPoint ugcPoint = SaveManager.ImportUGC(nameFiles[i]);
                ugcPoint.nameUGCPoint = Path.GetFileNameWithoutExtension(nameFiles[i]);
                UGCGUI ugcGUI = GameObject.Instantiate(prefabMode, content.transform).GetComponent<UGCGUI>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(ugcGUI.GetComponent<RectTransform>());
                ugcGUI.Init(this, ugcPoint, nameFiles[i]);
                ugcGUIs.Add(ugcGUI);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    public void DeleteUGCGUI(UGCGUI ugcGUI, string pathToFile)
    {
        File.Delete(pathToFile);

        GameObject.Destroy(ugcGUI.gameObject);
        ugcGUIs.Remove(ugcGUI);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    private void AddUGCGUI(UGCPoint ugcPoint, string pathToFile)
    {
        UGCGUI ugcGUI = GameObject.Instantiate(prefabMode, content.transform).GetComponent<UGCGUI>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(ugcGUI.GetComponent<RectTransform>());
        ugcGUI.Init(this, ugcPoint, pathToFile);
        Export(ugcPoint, pathToFile);
        ugcGUIs.Add(ugcGUI);
    }
    private UGCPoint FindUGCPointToFileUGCDatabase(string nameUGCPoint)
    {
        UGCPoint ugcPoint = null;

        for (int i = 0; i < ugcDatabase.ugcPoints.Count; i++)
        {
            if (ugcDatabase.ugcPoints[i].nameUGCPoint == nameUGCPoint)
            {
                ugcPoint = ugcDatabase.ugcPoints[i];
                break;
            }
        }
        return ugcPoint;
    }


    public void OpenDirectoryMod()
    {
        if (Directory.Exists(Path.GetFullPath(SaveManager.pathToDirectoryMod)))
        {
            Process.Start("explorer.exe", $@"{Path.GetFullPath(SaveManager.pathToDirectoryMod)}");
        }
        else
        {
            Directory.CreateDirectory(Path.GetFullPath(SaveManager.pathToDirectoryMod));
            Process.Start("explorer.exe", $@"{Path.GetFullPath(SaveManager.pathToDirectoryMod)}");
        }
    }
    public UGCPoint Import(string pathToFile)
    {
        return SaveManager.ImportUGC(pathToFile);
    }

    public void Export(UGCPoint ugcPoint, string pathToFile)
    {
        SaveManager.ExportUGC(ugcPoint, pathToFile);
    }
}
