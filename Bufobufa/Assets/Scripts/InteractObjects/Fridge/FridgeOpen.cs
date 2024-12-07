using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FridgeOpen : MonoBehaviour
{
    [SerializeField] private GameObject prefabMagnet;
    [SerializeField] private FileMagnets fileMagnets;
    [SerializeField] private SaveManager saveManager;
    public UnityEvent<Magnet> OnCreateMagnet;

    private bool OneTap = true;
    private GameObject FrontFridge;
    private List<MagnetGUI> magnetsGUI = new List<MagnetGUI>();

    private void Start()
    {
        FrontFridge = transform.Find("FrontFridge").gameObject;

        //for (int i = 0; i < fileMagnets.magnets.Count; i++)
        //{
        //    prefabMagnet.name = $"Magnet {i}";
        //    MagnetGUI magnetGUI = Instantiate(prefabMagnet, transform).GetComponent<MagnetGUI>();
        //    magnetGUI.Init(fileMagnets.magnets[i]);
        //    magnetsGUI.Add(magnetGUI);
        //}
    }

    private void Update()
    {
        if (GetComponent<OpenObject>().ObjectIsOpen && OneTap)
        {
            OneTap = false;
            FrontFridge.SetActive(true);
            for (int i = 0; i < magnetsGUI.Count; i++)
            {
                magnetsGUI[i].GetComponent<BoxCollider>().enabled = true;
                magnetsGUI[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else if (!GetComponent<OpenObject>().ObjectIsOpen && !OneTap)
        {
            OneTap = true;
            FrontFridge.SetActive(false);
            for (int i = 0; i < magnetsGUI.Count; i++)
            {
                magnetsGUI[i].GetComponent<BoxCollider>().enabled = false;
                magnetsGUI[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void ChangeMouseTrigger()
    {
        for (int i = 0; i < magnetsGUI.Count; i++)
        {
            if (magnetsGUI[i].GetComponent<MagnetMouseMove>().OnDrag)
            {
                magnetsGUI[i].GetComponent<MouseTrigger>().enabled = true;
            }
            else
            {
                magnetsGUI[i].GetComponent<MouseTrigger>().enabled = false;
            }
        }
    }

    public void OnMouseTrigger()
    {
        for (int i = 0; i < magnetsGUI.Count; i++)
        {
            magnetsGUI[i].GetComponent<MouseTrigger>().enabled = true;
        }
        saveManager.UpdatePlayerFile();
    }

    public void CreateMagnet(string typeMagnet)
    {
        for (int i = 0; i < fileMagnets.magnets.Count; i++)
        {
            if (fileMagnets.magnets[i].typeMagnet == typeMagnet)
            {
                MagnetGUI magnetGUI = Instantiate(prefabMagnet, transform).GetComponent<MagnetGUI>();
                magnetGUI.Init(fileMagnets.magnets[i]);
                magnetsGUI.Add(magnetGUI);
                OnCreateMagnet?.Invoke(magnetGUI.GetMagnet());

                MagnetSave magnetSave = new MagnetSave();
                magnetSave.typeMagnet = magnetGUI.GetMagnet().typeMagnet;
                magnetSave.x = magnetGUI.transform.position.x;
                magnetSave.y = magnetGUI.transform.position.y;
                magnetSave.z = magnetGUI.transform.position.z;

                print("1");

                saveManager.ChangeMagnetSave(magnetSave);

                return;
            }
        }
    }
}
