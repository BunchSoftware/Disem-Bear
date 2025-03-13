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
        [SerializeField] private FileMagnets fileMagnets;

        public UnityEvent OnFridgeOpen;
        public UnityEvent OnFridgeClose;

        public bool IsOpen => isOpen;
        private bool isOpen = false;

        private List<Magnet> magnets = new List<Magnet>();

        private Player player;
        private PlayerMouseMove playerMouseMove;

        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;
        private TriggerObject triggerObject;


        public void Init(Player player, PlayerMouseMove playerMouseMove)
        {
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();
            triggerObject = GetComponentInChildren<TriggerObject>();

            if (SaveManager.filePlayer.JSONPlayer.resources.magnets != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.magnets.Count; i++)
                {
                    prefabMagnet.name = $"Magnet {i}";
                    Magnet magnet = Instantiate(prefabMagnet, content.transform).GetComponent<Magnet>();

                    for (int j = 0; j < fileMagnets.magnets.Count; j++)
                    {
                        if (SaveManager.filePlayer.JSONPlayer.resources.magnets[i].typeMagnet == fileMagnets.magnets[j].typeMagnet)
                        {
                            MagnetInfo magnetInfo = new MagnetInfo()
                            {
                                x = SaveManager.filePlayer.JSONPlayer.resources.magnets[i].x,
                                y = SaveManager.filePlayer.JSONPlayer.resources.magnets[i].y,
                                z = SaveManager.filePlayer.JSONPlayer.resources.magnets[i].z,
                                typeMagnet = SaveManager.filePlayer.JSONPlayer.resources.magnets[i].typeMagnet,
                                iconMagnet = fileMagnets.magnets[j].iconMagnet,
                            };
                            magnet.Init(this, magnetInfo);
                            break;
                        }
                    }
                    magnets.Add(magnet);
                }
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
        }

        public void OnUpdate(float deltatime)
        {
            openObject.OnUpdate(deltatime);
        }

        public void CreateMagnet(string typeMagnet)
        {
            for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.magnets.Count; i++)
            {
                if (SaveManager.filePlayer.JSONPlayer.resources.magnets[i].typeMagnet == typeMagnet)
                {
                    Magnet magnet = Instantiate(prefabMagnet, content.transform).GetComponent<Magnet>();

                    MagnetInfo magnetInfo = new MagnetInfo();
                    magnetInfo.typeMagnet = SaveManager.filePlayer.JSONPlayer.resources.magnets[i].typeMagnet;
                    magnetInfo.x = SaveManager.filePlayer.JSONPlayer.resources.magnets[i].x;
                    magnetInfo.y = SaveManager.filePlayer.JSONPlayer.resources.magnets[i].y;
                    magnetInfo.z = SaveManager.filePlayer.JSONPlayer.resources.magnets[i].z;

                    magnet.Init(this, magnetInfo);
                    magnets.Add(magnet);

                    MagnetData magnetSave = new MagnetData();
                    magnetSave.typeMagnet = magnet.GetMagnet().typeMagnet;
                    magnetSave.x = magnet.transform.localPosition.x;
                    magnetSave.y = magnet.transform.localPosition.y;
                    magnetSave.z = magnet.transform.localPosition.z;

                    SaveManager.ChangeMagnetSave(magnetSave);

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
