using Assets.Scripts.Environment.Aquarium;
using External.DI;
using External.Storage;
using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.Environment.LModelBoard;
using Game.Environment.LPostTube;
using Game.Environment.LTableWithItems;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.Aquarium
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class Aquarium : MonoBehaviour, ILeftMouseDownClickable
    {
        [SerializeField] private AquariumMaterialDatabase aquariumMaterialDatabase;
        [SerializeField] private TriggerObject triggerObject;
        [SerializeField] private ParticleSystem particleSystem;

        [SerializeField] private SpriteRenderer aquariumRenderer;
        [SerializeField] private SpriteRenderer choiceCellSprite;
        [SerializeField] private MovePointToPoint spriteMovePointToPoint;
        [SerializeField] private GameObject DisplayCount;

        [SerializeField] private ChangeCell buttonLeft;
        [SerializeField] private ChangeCell buttonRight;

        [SerializeField] private List<ColorAquarium> InspectorPhasesAquariums = new();
        private Dictionary<string, PhasesAquarium> phasesAquariums = new();
        [SerializeField] private List<TimeCell> InspectorTimeCells = new();
        private Dictionary<string, float> timeCells = new();
        [SerializeField] private List<IngradientSpawner> InspectorSpawners = new();
        private Dictionary<string, IngradientSpawner> Spawners = new();
        [Header("GetCellsSounds")]
        [SerializeField] private List<AudioClip> getCellsSounds = new();
        [Header("ChangeMaterialSounds")]
        [SerializeField] private List<AudioClip> changeMaterialSounds = new();


        public UnityEvent<string, int> GetAquariumCells;
        public UnityEvent OnAquariumOpen;
        public UnityEvent OnAquariumClose;
        public UnityEvent OnAquariumBecomeDirty;

        private Player player;
        private PlayerMouseMove playerMouseMove;
        private MaterialForAquarium currentMaterialForAquarium;

        private bool isClick = false;
        private bool isOpen = false;
        public bool IsOpen => isOpen;

        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;
        private GameBootstrap gameBootstrap;

        private List<string> currentCells = new();
        private float timeMaterial = 0f;
        private float spendTimeCreateCell = 0f;

        private int indexCell = 0;
        private int countCell = 0;
        private bool aquariumDirty = false;

        public bool on = false;

        public void Init(Player player, PlayerMouseMove playerMouseMove, GameBootstrap gameBootstrap)
        {
            this.player = player;
            this.playerMouseMove = playerMouseMove;
            this.gameBootstrap = gameBootstrap;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            if (triggerObject == null)
                Debug.LogError("�� ����� ������� � ���������");

            openObject.OnStartObjectOpen.AddListener(() =>
            {
                spriteMovePointToPoint.StartMoveTo(openObject.timeOpen);
                scaleChooseObject.on = false;
            });

            openObject.OnEndObjectOpen.AddListener(() =>
            {
                isOpen = true;
                OnAquariumOpen?.Invoke();
            });

            openObject.OnStartObjectClose.AddListener(() =>
            {
                spriteMovePointToPoint.StartMoveTo(openObject.timeClose);
            });

            openObject.OnEndObjectClose.AddListener(() =>
            {
                isOpen = false;
                scaleChooseObject.on = true;
                OnAquariumClose?.Invoke();
            });

            openObject.Init(triggerObject, playerMouseMove, player);

            if(SaveManager.playerDatabase.JSONPlayer.resources.aquariums == null)
            {
                SaveManager.playerDatabase.JSONPlayer.resources.aquariums = new List<AquariumData>();

                AquariumData aquariumData = new AquariumData();
                aquariumData.nameMasterAquarium = transform.parent.name;
                aquariumData.countCell = 0;
                aquariumData.colorMaterial = "";

                SaveManager.playerDatabase.JSONPlayer.resources.aquariums.Add(aquariumData);

            }
            else
            {
                bool condition = true;
                for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.aquariums.Count; i++)
                {
                    if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].nameMasterAquarium == transform.parent.name)
                    {
                        condition = false;
                        break;
                    }
                }

                if (condition)
                {
                    AquariumData aquariumData = new AquariumData();
                    aquariumData.nameMasterAquarium = transform.parent.name;
                    aquariumData.countCell = 0;
                    aquariumData.colorMaterial = "";

                    SaveManager.playerDatabase.JSONPlayer.resources.aquariums.Add(aquariumData);
                }
                else
                {
                    if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums != null)
                    {
                        for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.aquariums.Count; i++)
                        {
                            if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].nameMasterAquarium == transform.parent.name)
                            {
                                currentMaterialForAquarium = FindMaterialForAquariumToDatabase(SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].colorMaterial);
                                countCell = SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].countCell;
                            }
                        }
                    }
                }
            }

            SaveManager.UpdatePlayerDatabase();

            triggerObject.OnTriggerStayEvent.AddListener((collider) =>
            {
                if (isClick && !isOpen)
                {
                    isClick = false;

                    if (player.PlayerPickUpItem && TryGetMaterial(player.GetPickUpItem()))
                    {
                        Destroy(player.GetPickUpItem().gameObject);
                        player.PutItem();
                        Debug.Log("Material for aquarium update");
                    }
                }
                else if (isClick && isOpen)
                {
                    isClick = false;

                    GetAllCells();
                }
            });

            for (int i = 0; i < InspectorSpawners.Count; i++)
            {
                Spawners[InspectorSpawners[i].GetIngradient().typeIngradient] = InspectorSpawners[i];
            }
            for (int i = 0; i < InspectorPhasesAquariums.Count; i++)
            {
                phasesAquariums[InspectorPhasesAquariums[i].nameColor] = InspectorPhasesAquariums[i].phasesAquarium;
            }
            for (int i = 0; i < InspectorTimeCells.Count; i++)
            {
                timeCells[InspectorTimeCells[i].name] = InspectorTimeCells[i].time;
            }

            GetAquariumCells.AddListener((nameCell, countCell) =>
            {
                gameBootstrap.OnPlayOneShotRandomSound(getCellsSounds);
            });

            buttonLeft.Init(this);
            buttonRight.Init(this);

            QuietUpdateData(currentMaterialForAquarium);

            Debug.Log("Aquarium: ������� ��������������");
        }

        private MaterialForAquarium FindMaterialForAquariumToDatabase(string colorMaterial)
        {
            for (int i = 0; i < aquariumMaterialDatabase.materialForAquariums.Count; i++)
            {
                if (aquariumMaterialDatabase.materialForAquariums[i].colorMaterial == colorMaterial)
                    return aquariumMaterialDatabase.materialForAquariums[i];
            }

            return null;
        }

        public void OnUpdate(float deltaTime)
        {
            if (phasesAquariums.ContainsKey(currentMaterialForAquarium.colorMaterial))
            {
                if (countCell == 0)
                {
                    aquariumRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[currentMaterialForAquarium.colorMaterial].NullFaseDirty : phasesAquariums[currentMaterialForAquarium.colorMaterial].NullFase;
                }
                else if (countCell < 4)
                {
                    aquariumRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[currentMaterialForAquarium.colorMaterial].FirstFaseDirty : phasesAquariums[currentMaterialForAquarium.colorMaterial].FirstFase;
                }
                else if (countCell < 9)
                {
                    aquariumRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[currentMaterialForAquarium.colorMaterial].SecondFaseDirty : phasesAquariums[currentMaterialForAquarium.colorMaterial].SecondFase;
                }
                else if (countCell < 15)
                {
                    aquariumRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[currentMaterialForAquarium.colorMaterial].ThirdFaseDirty : phasesAquariums[currentMaterialForAquarium.colorMaterial].ThirdFase;
                }
            }
            if (on) {
                if (timeMaterial > 0f)
                {
                    aquariumDirty = false;
                    timeMaterial -= Time.deltaTime;
                }
                
                if (timeMaterial > 0f) spendTimeCreateCell += Time.deltaTime;
                else if (aquariumDirty == false)
                {
                    aquariumDirty = true;
                    OnAquariumBecomeDirty?.Invoke();
                }
                if (spendTimeCreateCell >= timeCells[currentCells[indexCell]])
                {
                    if (countCell < 15)
                    {
                        countCell++;
                        if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums != null)
                        {
                            for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.aquariums.Count; i++)
                            {
                                if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].nameMasterAquarium == transform.parent.name)
                                {
                                    SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].countCell = countCell;
                                }
                            }
                        }

                        SaveManager.UpdatePlayerDatabase();
                    }
                    spendTimeCreateCell = 0;
                }
            }
            if (openObject != null)
                openObject.OnUpdate(deltaTime);
        }

        private bool TryGetMaterial(PickUpItem pickUpItem)
        {
            if (pickUpItem != null)
            {
                MaterialForAquarium materialForAquarium;
                switch (pickUpItem.TypeItem)
                {
                    case TypePickUpItem.None:
                        break;
                    case TypePickUpItem.AquariumMaterial:

                        if (pickUpItem.TryGetComponent(out materialForAquarium))
                        {
                            UpdateData(materialForAquarium);
                            return true;
                        }
                        else
                        {
                            Debug.LogError("������ ����� ��� �������� ���������, �� �� ����� ������� MaterialForAquarium");
                        }
                        break;
                    case TypePickUpItem.Package:
                        PackageItem packageItem;
                        if (pickUpItem.TryGetComponent(out packageItem))
                        {
                            if (packageItem.itemInPackage.TryGetComponent(out materialForAquarium))
                            {
                                UpdateData(materialForAquarium);
                                return true;
                            }
                            else
                            {
                                Debug.Log("����� � �������� �������, ��� ��� ��� ������� MaterialForAquarium");
                            }
                        }
                        else
                        {
                            Debug.LogError("������. �� ������� ��� PackageItem, �� ������ ������ ��� Package");
                        }
                        break;
                }
            }
            return false;
        }


        private void QuietUpdateData(MaterialForAquarium materialForAquarium)
        {
            currentMaterialForAquarium = materialForAquarium;
            if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums != null)
            {
                for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.aquariums.Count; i++)
                {
                    if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].nameMasterAquarium == transform.parent.name)
                    {
                        SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].colorMaterial = materialForAquarium.colorMaterial;
                    }
                }
            }

            SaveManager.UpdatePlayerDatabase();
            if (currentMaterialForAquarium != null)
            {
                currentCells = new List<string>(currentMaterialForAquarium.cells);
                if (currentCells.Count > 1)
                {
                    buttonLeft.SetOn();
                    buttonRight.SetOn();
                }
                else
                {
                    buttonLeft.SetOff();
                    buttonRight.SetOff();
                }
                timeMaterial = currentMaterialForAquarium.TimeMaterial;
                indexCell = 0;
                choiceCellSprite.sprite = Spawners[currentCells[indexCell]].GetSpriteIngradient();
                spendTimeCreateCell = 0;
            }
        }

        private void UpdateData(MaterialForAquarium materialForAquarium)
        {
            currentMaterialForAquarium = materialForAquarium;
            if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums != null)
            {
                for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.aquariums.Count; i++)
                {
                    if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].nameMasterAquarium == transform.parent.name)
                    {
                        SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].colorMaterial = materialForAquarium.colorMaterial;
                    }
                }
            }

            SaveManager.UpdatePlayerDatabase();
            if (currentMaterialForAquarium != null)
            {
                gameBootstrap.OnPlayOneShotRandomSound(changeMaterialSounds);
                currentCells = new List<string>(currentMaterialForAquarium.cells);
                if (currentCells.Count > 1)
                {
                    buttonLeft.SetOn();
                    buttonRight.SetOn();
                }
                else
                {
                    buttonLeft.SetOff();
                    buttonRight.SetOff();
                }
                timeMaterial = currentMaterialForAquarium.TimeMaterial;
                GetAllCells();
                indexCell = 0;
                choiceCellSprite.sprite = Spawners[currentCells[indexCell]].GetSpriteIngradient();
                spendTimeCreateCell = 0;
            }
        }

        public void ChangeCellRight()
        {
            GetAllCells();
            indexCell = (indexCell + 1 + currentCells.Count) % currentCells.Count;
            choiceCellSprite.sprite = Spawners[currentCells[indexCell]].GetSpriteIngradient();
            spendTimeCreateCell = 0;
        }

        public void ChangeCellLeft()
        {
            GetAllCells();
            indexCell = (indexCell - 1 + currentCells.Count) % currentCells.Count;
            choiceCellSprite.sprite = Spawners[currentCells[indexCell]].GetSpriteIngradient();
            spendTimeCreateCell = 0;
        }

        private void GetAllCells()
        {
            if (countCell != 0)
            {
                particleSystem.Play();
                StartCoroutine(WaitParticleSystem(0.3f));
                GetAquariumCells?.Invoke(currentCells[indexCell], countCell);
                DisplayCount.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = countCell.ToString();
                DisplayCount.GetComponent<Animator>().SetBool("On", true);
                StartCoroutine(waitDisplayCount());

                Spawners[currentCells[indexCell]].PutIngradient(countCell);

                countCell = 0;

                if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums != null)
                {
                    for (int i = 0; i < SaveManager.playerDatabase.JSONPlayer.resources.aquariums.Count; i++)
                    {
                        if (SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].nameMasterAquarium == transform.parent.name)
                        {
                            SaveManager.playerDatabase.JSONPlayer.resources.aquariums[i].countCell = 0;
                        }
                    }
                }

                SaveManager.UpdatePlayerDatabase();
            }
        }

        private IEnumerator WaitParticleSystem(float f)
        {
            yield return new WaitForSeconds(f);
            particleSystem.Stop();
        }

        private IEnumerator waitDisplayCount()
        {
            yield return new WaitForSeconds(2);
            DisplayCount.GetComponent<Animator>().SetBool("On", false);
        }

        public void OnMouseLeftClickDownObject()
        {
            isClick = true;
        }

        public void OnMouseLeftClickDownOtherObject()
        {
            isClick = false;
        }

        [Serializable]
        public class TimeCell
        {
            public string name;
            public float time;
        }
    }
    [Serializable]
    public class ColorAquarium
    {
        public string nameColor = "none";

        public PhasesAquarium phasesAquarium;
    }

    [Serializable]
    public class PhasesAquarium
    {
        public Sprite NullFase;
        public Sprite FirstFase;
        public Sprite SecondFase;
        public Sprite ThirdFase;
        public Sprite NullFaseDirty;
        public Sprite FirstFaseDirty;
        public Sprite SecondFaseDirty;
        public Sprite ThirdFaseDirty;
    }
}
