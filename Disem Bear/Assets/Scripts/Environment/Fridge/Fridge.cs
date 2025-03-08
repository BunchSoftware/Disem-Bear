using External.Storage;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.Fridge
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class Fridge : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject prefabMagnet;

        public UnityEvent OnFridgeOpen;
        public UnityEvent OnFridgeClose;

        public bool IsOpen => isOpen;
        private bool isOpen = false;

        public List<Magnet> magnets = new List<Magnet>();

        private SaveManager saveManager;
        private Player player;
        private PlayerMouseMove playerMouseMove;

        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;
        private TriggerObject triggerObject;


        public void Init(SaveManager saveManager, Player player, PlayerMouseMove playerMouseMove)
        {
            this.saveManager = saveManager;
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();
            triggerObject = GetComponentInChildren<TriggerObject>();

            for (int i = 0; i < magnets.Count; i++)
            {
                magnets[i].Init(this, saveManager, null);
            }

            content.GetComponent<Collider>().enabled = false;
            for (int i = 0; i < content.transform.childCount; i++)
            {
                content.transform.GetChild(i).GetComponent<Collider>().enabled = false;
            }

            openObject.OnEndObjectOpen.AddListener(() =>
            {
                isOpen = true;
                scaleChooseObject.on = false;
                OnFridgeOpen?.Invoke();
                GetComponent<Collider>().enabled = false;

                content.GetComponent<Collider>().enabled = true;
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    content.transform.GetChild(i).GetComponent<Collider>().enabled = true;
                }
            });

            openObject.OnStartObjectClose.AddListener(() =>
            {
                isOpen = false;
                content.GetComponent<Collider>().enabled = false;
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    content.transform.GetChild(i).GetComponent<Collider>().enabled = false;
                }
            });
            openObject.OnEndObjectClose.AddListener(() =>
            {
                scaleChooseObject.on = true;
                OnFridgeClose?.Invoke();
                GetComponent<Collider>().enabled = true;
            });
            openObject.Init(triggerObject, playerMouseMove, player);

            Debug.Log("Fridge: Успешно иницилизирован");

            //FrontFridge = transform.Find("FrontFridge").gameObject;

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

        public void OnUpdate(float deltatime)
        {
            openObject.OnUpdate(deltatime);
        }

        public void CreateMagnet(string typeMagnet)
        {
            for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.magnetSaves.Count; i++)
            {
                if (saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].typeMagnet == typeMagnet)
                {
                    Magnet magnet = Instantiate(prefabMagnet, content.transform).GetComponent<Magnet>();

                    MagnetInfo magnetInfo = new MagnetInfo();
                    magnetInfo.typeMagnet = saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].typeMagnet;
                    magnetInfo.x = saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].x;
                    magnetInfo.y = saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].y;
                    magnetInfo.z = saveManager.filePlayer.JSONPlayer.resources.magnetSaves[i].z;

                    magnet.Init(this, saveManager, magnetInfo);
                    magnets.Add(magnet);

                    MagnetSave magnetSave = new MagnetSave();
                    magnetSave.typeMagnet = magnet.GetMagnet().typeMagnet;
                    magnetSave.x = magnet.transform.localPosition.x;
                    magnetSave.y = magnet.transform.localPosition.y;
                    magnetSave.z = magnet.transform.localPosition.z;

                    saveManager.ChangeMagnetSave(magnetSave);

                    return;
                }
            }
        }

        public void CreateMagnet(Reward reward)
        {
            CreateMagnet(reward.typeReward);
        }
    }
}
