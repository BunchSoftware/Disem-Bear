using External.DI;
using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.Environment.LPostTube;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.Aquarium
{
    [RequireComponent(typeof(OpenObject))]
    [RequireComponent(typeof(ScaleChooseObject))]
    public class Aquarium : MonoBehaviour, ILeftMouseDownClickable
    {
        [SerializeField] private TriggerObject triggerObject;
        [SerializeField] private ParticleSystem particleSystem;

        [SerializeField] private SpriteRenderer aquariumRenderer;
        [SerializeField] private SpriteRenderer choiceCellSprite;
        [SerializeField] private MovePointToPoint spriteMovePointToPoint;
        [SerializeField] private GameObject DisplayCount;

        [SerializeField] private ChangeCell buttonLeft;
        [SerializeField] private ChangeCell buttonRight;

        [SerializeField] private MaterialForAquarium materialForAquarium;

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

        private Player player;
        private PlayerMouseMove playerMouseMove;

        private bool isClick = false;
        private bool isOpen = false;
        public bool IsOpen => isOpen;

        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;
        private GameBootstrap gameBootstrap;

        private List<string> currentCells = new();
        private string colorMaterial = "none";
        private float timeMaterial = 0f;
        private float spendTimeCreateCell = 0f;

        private int indexCell = 0;
        private int countCells = 0;

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
                isOpen = true;
                spriteMovePointToPoint.StartMoveTo(openObject.timeOpen);
                scaleChooseObject.on = false;
            });

            openObject.OnEndObjectOpen.AddListener(() =>
            {
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

            QuietUpdateData(materialForAquarium);

            Debug.Log("Aquarium: ������� ��������������");
        }

        public void OnUpdate(float deltaTime)
        {
            if (timeMaterial > 0f)
            {
                timeMaterial -= Time.deltaTime;
            }
            if (phasesAquariums.ContainsKey(colorMaterial))
            {
                if (countCells == 0)
                {
                    aquariumRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[colorMaterial].NullFaseDirty : phasesAquariums[colorMaterial].NullFase;
                }
                else if (countCells < 4)
                {
                    aquariumRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[colorMaterial].FirstFaseDirty : phasesAquariums[colorMaterial].FirstFase;
                }
                else if (countCells < 9)
                {
                    aquariumRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[colorMaterial].SecondFaseDirty : phasesAquariums[colorMaterial].SecondFase;
                }
                else if (countCells < 15)
                {
                    aquariumRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[colorMaterial].ThirdFaseDirty : phasesAquariums[colorMaterial].ThirdFase;
                }
            }
            if (timeMaterial > 0f) spendTimeCreateCell += Time.deltaTime;
            if (spendTimeCreateCell >= timeCells[currentCells[indexCell]])
            {
                if (countCells < 15)
                    countCells++;
                spendTimeCreateCell = 0;
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
            if (materialForAquarium != null)
            {
                currentCells = new List<string>(materialForAquarium.cells);
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
                colorMaterial = materialForAquarium.colorMaterial;
                timeMaterial = materialForAquarium.TimeMaterial;
                indexCell = 0;
                choiceCellSprite.sprite = Spawners[currentCells[indexCell]].GetSpriteIngradient();
                spendTimeCreateCell = 0;
            }
        }

        private void UpdateData(MaterialForAquarium materialForAquarium)
        {
            if (materialForAquarium != null)
            {
                gameBootstrap.OnPlayOneShotRandomSound(changeMaterialSounds);
                currentCells = new List<string>(materialForAquarium.cells);
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
                colorMaterial = materialForAquarium.colorMaterial;
                timeMaterial = materialForAquarium.TimeMaterial;
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

        private void OnMouseDown()
        {
            GetAllCells();
        }

        private void GetAllCells()
        {
            if (countCells != 0)
            {
                particleSystem.Play();
                StartCoroutine(WaitParticleSystem(0.3f));
                GetAquariumCells?.Invoke(currentCells[indexCell], countCells);
                DisplayCount.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = countCells.ToString();
                DisplayCount.GetComponent<Animator>().SetBool("On", true);
                StartCoroutine(waitDisplayCount());

                Spawners[currentCells[indexCell]].PutIngradient(countCells);

                countCells = 0;
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
