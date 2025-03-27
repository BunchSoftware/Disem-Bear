using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EditReciepePanel : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject prefabEditReciepeUI;
    [SerializeField] private Button addNewReciepe;
    [SerializeField] private Button deleteAllReciepe;


    private List<EditReciepeGUI> editReciepeGUIs = new List<EditReciepeGUI>();
    private string pathToFile;
    private UGCPoint ugcPoint;
    private UGCManager ugcManager;
    private UGCGUI ugcGUI;
    public void Init(UGCManager ugcManager)
    {
        this.ugcManager = ugcManager;

        addNewReciepe.onClick.AddListener(() =>
        {
            AddEditReciepeGUI(new UGCReciep(), this.pathToFile);
        });
        deleteAllReciepe.onClick.AddListener(() =>
        {
            for (int i = 0; i < editReciepeGUIs.Count; i++)
            {
                Destroy(editReciepeGUIs[i].gameObject);
            }
            editReciepeGUIs.Clear();

            File.Delete(pathToFile);
            ugcPoint.ugcRecieps.Clear();
            ugcManager.Export(ugcPoint, pathToFile);
        });
    }
    public void UpdateData(UGCPoint ugcPoint, UGCGUI ugcGUI, string pathToFile)
    {
        this.pathToFile = pathToFile;
        this.ugcPoint = ugcPoint;
        this.ugcGUI = ugcGUI;
        for (int i = 0; i < editReciepeGUIs.Count; i++)
        {
            Destroy(editReciepeGUIs[i].gameObject);
        }
        editReciepeGUIs.Clear();
        for (int i = 0; i < ugcPoint.ugcRecieps.Count; i++)
        {
            EditReciepeGUI editReciepeGUI = Instantiate(prefabEditReciepeUI, content.transform).GetComponent<EditReciepeGUI>();
            editReciepeGUI.Init(this, ugcPoint.ugcRecieps[i], pathToFile);
            editReciepeGUIs.Add(editReciepeGUI);
            LayoutRebuilder.ForceRebuildLayoutImmediate(editReciepeGUI.GetComponent<RectTransform>());
        }

        ugcGUI.UpdateData(ugcPoint);

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    public void DeleteReciepGUI(EditReciepeGUI editReciepGUI)
    {
        File.Delete(pathToFile);
        ugcPoint.ugcRecieps.Remove(editReciepGUI.GetUGCReciep());
        ugcManager.Export(ugcPoint, pathToFile);

        Destroy(editReciepGUI.gameObject);
        editReciepeGUIs.Remove(editReciepGUI);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }
    private void AddEditReciepeGUI(UGCReciep ugcReciepe, string pathToFile)
    {
        EditReciepeGUI editReciepeGUI = Instantiate(prefabEditReciepeUI, content.transform).GetComponent<EditReciepeGUI>();
        editReciepeGUI.Init(this, ugcReciepe, pathToFile);

        LayoutRebuilder.ForceRebuildLayoutImmediate(editReciepeGUI.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        File.Delete(pathToFile);
        ugcPoint.ugcRecieps.Add(ugcReciepe);
        ugcManager.Export(ugcPoint, pathToFile);

        editReciepeGUIs.Add(editReciepeGUI);
    }
}
