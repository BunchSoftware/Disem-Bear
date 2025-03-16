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
        private Bounds dragBounds;

        public void Init(Player player, PlayerMouseMove playerMouseMove)
        {
            this.player = player;
            this.playerMouseMove = playerMouseMove;

            dragBounds = content.GetComponent<Collider>().bounds;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();
            triggerObject = GetComponentInChildren<TriggerObject>();

            if (SaveManager.filePlayer.JSONPlayer.resources.magnets != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.magnets.Count; i++)
                {
                    Magnet magnet = Instantiate(prefabMagnet, content.transform).GetComponent<Magnet>();
                    magnet.name = $"Magnet {content.transform.childCount - 1}";
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
                            magnet.Init(this, magnetInfo, dragBounds);
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
            for (int i = 0; i < fileMagnets.magnets.Count; i++)
            {
                if (fileMagnets.magnets[i].typeMagnet == typeMagnet)
                {
                    Magnet magnet = Instantiate(prefabMagnet, content.transform).GetComponent<Magnet>();
                    magnet.name = $"Magnet {content.transform.childCount - 1}";

                    MagnetInfo magnetInfo = new MagnetInfo();
                    magnetInfo.iconMagnet = fileMagnets.magnets[i].iconMagnet;
                    magnetInfo.typeMagnet = fileMagnets.magnets[i].typeMagnet;

                    Bounds boundsMagnet = magnet.GetComponent<Collider>().bounds;

                    float y = Random.Range(dragBounds.min.y, dragBounds.max.y);
                    float z = 0;

                    magnetInfo.x = 0;
                    magnetInfo.y = y;
                    magnetInfo.z = z;

                    magnet.Init(this, magnetInfo, dragBounds);
                    magnets.Add(magnet);

                    MagnetData magnetSave = new MagnetData();
                    magnetSave.typeMagnet = magnet.GetMagnet().typeMagnet;
                    magnetSave.x = magnet.transform.localPosition.x;
                    magnetSave.y = magnet.transform.localPosition.y;
                    magnetSave.z = magnet.transform.localPosition.z;

                    SaveManager.ChangeMagnetSave(magnetSave);
                    Debug.Log($"Магнит {typeMagnet} создан");

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
