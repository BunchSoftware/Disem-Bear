using External.Storage;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.Fridge
{
    public class FridgeOpen : MonoBehaviour
    {
        [SerializeField] private GameObject prefabMagnet;
        [SerializeField] private FileMagnets fileMagnets;
        [SerializeField] private SaveManager saveManager;
        public UnityEvent<Magnet> OnCreateMagnet;

        private bool OneTap = true;
        private GameObject FrontFridge;
        public List<MagnetGUI> magnetsGUI = new List<MagnetGUI>();

        private void Start()
        {
            FrontFridge = transform.Find("FrontFridge").gameObject;

            //if (saveManager.filePlayer.JSONPlayer.resources.magnetSaves != null)
            //{
            //    for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.magnetSaves.Count; i++)
            //    {
            //        prefabMagnet.name = $"Magnet {i}";
            //        MagnetGUI magnetGUI = Instantiate(prefabMagnet, transform).GetComponent<MagnetGUI>();

            //        for (int j = 0; j < fileMagnets.magnets.Count; j++)
            //        {
            //            if (saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].typeMagnet == fileMagnets.magnets[j].typeMagnet)
            //            {
            //                Magnet magnet = new Magnet()
            //                {
            //                    x = saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].x,
            //                    y = saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].y,
            //                    z = saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].z,
            //                    typeMagnet = saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].typeMagnet,
            //                    iconMagnet = fileMagnets.magnets[j].iconMagnet,
            //                };
            //                magnetGUI.Init(magnet);
            //                break;
            //            }
            //        }
            //        magnetsGUI.Add(magnetGUI);
            //    }
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

                MagnetSave magnetSave = new MagnetSave();
                magnetSave.typeMagnet = magnetsGUI[i].GetMagnet().typeMagnet;
                magnetSave.x = magnetsGUI[i].transform.localPosition.x;
                magnetSave.y = magnetsGUI[i].transform.localPosition.y;
                magnetSave.z = magnetsGUI[i].transform.localPosition.z;

                saveManager.ChangeMagnetSave(magnetSave);
            }
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
                    magnetSave.x = magnetGUI.transform.localPosition.x;
                    magnetSave.y = magnetGUI.transform.localPosition.y;
                    magnetSave.z = magnetGUI.transform.localPosition.z;

                    saveManager.ChangeMagnetSave(magnetSave);

                    return;
                }
            }
        }
    }
}
