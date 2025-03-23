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
        private OpenObject openObject;
        private ScaleChooseObject scaleChooseObject;
        [SerializeField] private TriggerObject triggerObject;
        public MaterialForAquarium materialForAquarium;
        private List<string> currentCells = new();
        private string colorMaterial = "none";
        private float timeMaterial = 0f;
        private float spendTimeCreateCell = 0f;

        [SerializeField] private List<ColorAquarium> InspectorPhasesAquariums = new();
        private Dictionary<string, PhasesAquarium> phasesAquariums = new();
        [SerializeField] private List<TimeCell> InspectorTimeCells = new();
        private Dictionary<string, float> timeCells = new();
        [SerializeField] private List<IngradientSpawner> InspectorSpawners = new();
        private Dictionary<string, IngradientSpawner> Spawners = new();
        private int NumCell = 0;
        private SpriteRenderer ChoiceCellSprite;
        private SpriteRenderer spriteRenderer;
        private ParticleSystem particleSystem;
        private GameBootstrap gameBootstrap;
        private GameObject DisplayCount;
        [SerializeField] private ChangeCell buttonLeft;
        [SerializeField] private ChangeCell buttonRight;
        [Header("GetCellsSounds")]
        [SerializeField] private List<AudioClip> getCellsSounds = new();
        [Header("ChangeMaterialSounds")]
        [SerializeField] private List<AudioClip> changeMaterialSounds = new();

        private int CountCells = 0;

        public UnityEvent<string, int> GetAquariumCells;

        public UnityEvent OnAquariumOpen;
        public UnityEvent OnAquariumClose;

        private Player player;
        private PlayerMouseMove playerMouseMove;
        private MovePointToPoint spriteMovePointToPoint;

        private bool isClick = false;
        private bool isOpen = false;
        public bool IsOpen => isOpen;


        public void Init(Player player, PlayerMouseMove playerMouseMove, GameBootstrap gameBootstrap)
        {
            this.player = player;
            this.playerMouseMove = playerMouseMove;
            this.gameBootstrap = gameBootstrap;

            openObject = GetComponent<OpenObject>();
            scaleChooseObject = GetComponent<ScaleChooseObject>();

            if (triggerObject == null)
                Debug.LogError("Не задан триггер у аквариума");
            spriteMovePointToPoint = transform.Find("AquariumSprite").GetComponent<MovePointToPoint>();

            openObject.OnStartObjectOpen.AddListener(() =>
            {
                isOpen = true;
                spriteMovePointToPoint.StartMoveTo(openObject.timeOpen);
                scaleChooseObject.on = false;
            });

            openObject.OnEndObjectOpen.AddListener(() =>
            {
                collider.enabled = false;
                OnAquariumOpen?.Invoke();
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
            particleSystem = transform.Find("Particle System").GetComponent<ParticleSystem>();
            spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
            ChoiceCellSprite = transform.Find("ChoiceCell").GetComponent<SpriteRenderer>();
            DisplayCount = transform.Find("DisplayCount").gameObject;

            GetAquariumCells.AddListener((nameCell, countCell) =>
            {
                gameBootstrap.OnPlayOneShotRandomSound(getCellsSounds);
            });

            QuietUpdateMaterial(materialForAquarium);

            buttonLeft.Init(this);
            buttonRight.Init(this);

            Debug.Log("Aquarium: Успешно иницилизирован");
        }

        public void OnUpdate(float deltaTime)
        {
            if (timeMaterial > 0f)
            {
                timeMaterial -= Time.deltaTime;
            }
            if (phasesAquariums.ContainsKey(colorMaterial))
            {
                if (CountCells == 0)
                {
                    spriteRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[colorMaterial].NullFaseDirty : phasesAquariums[colorMaterial].NullFase;
                }
                else if (CountCells < 4)
                {
                    spriteRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[colorMaterial].FirstFaseDirty : phasesAquariums[colorMaterial].FirstFase;
                }
                else if (CountCells < 9)
                {
                    spriteRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[colorMaterial].SecondFaseDirty : phasesAquariums[colorMaterial].SecondFase;
                }
                else if (CountCells < 15)
                {
                    spriteRenderer.sprite = timeMaterial <= 0f ? phasesAquariums[colorMaterial].ThirdFaseDirty : phasesAquariums[colorMaterial].ThirdFase;
                }
            }
            if (timeMaterial > 0f) spendTimeCreateCell += Time.deltaTime;
            if (spendTimeCreateCell >= timeCells[currentCells[NumCell]])
            {
                if (CountCells < 15)
                    CountCells++;
                spendTimeCreateCell = 0;
            }

            if (openObject != null)
                openObject.OnUpdate(deltaTime);
            if (aquarium != null)
                aquarium.OnUpdate(deltaTime);
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
                            aquarium.UpdateMaterial(materialForAquarium);
                            return true;
                        }
                        else
                        {
                            Debug.LogError("Объект задан как материал аквариума, но не имеет скрипта MaterialForAquarium");
                        }
                        break;
                    case TypePickUpItem.Package:
                        PackageItem packageItem;
                        if (pickUpItem.TryGetComponent(out packageItem))
                        {
                            if (packageItem.itemInPackage.TryGetComponent(out materialForAquarium))
                            {
                                aquarium.UpdateMaterial(materialForAquarium);
                                return true;
                            }
                            else
                            {
                                Debug.Log("Отказ в принятии посылки, так как нет скрипта MaterialForAquarium");
                            }
                        }
                        else
                        {
                            Debug.LogError("Ошибка. На обьекте нет PackageItem, но обьект указан как Package");
                        }
                        break;
                }
            }
            return false;
        }

        public void QuietUpdateMaterial(MaterialForAquarium materialForAquarium)
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
                CountCells = 0;
                NumCell = 0;
                ChoiceCellSprite.sprite = Spawners[currentCells[NumCell]].GetSpriteIngradient();
                spendTimeCreateCell = 0;
            }
        }

        public void UpdateMaterial(MaterialForAquarium materialForAquarium)
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
                NumCell = 0;
                ChoiceCellSprite.sprite = Spawners[currentCells[NumCell]].GetSpriteIngradient();
                spendTimeCreateCell = 0;
            }
        }

        public void ChangeCellRight()
        {
            GetAllCells();
            NumCell = (NumCell + 1 + currentCells.Count) % currentCells.Count;
            ChoiceCellSprite.sprite = Spawners[currentCells[NumCell]].GetSpriteIngradient();
            spendTimeCreateCell = 0;
        }

        public void ChangeCellLeft()
        {
            GetAllCells();
            NumCell = (NumCell - 1 + currentCells.Count) % currentCells.Count;
            ChoiceCellSprite.sprite = Spawners[currentCells[NumCell]].GetSpriteIngradient();
            spendTimeCreateCell = 0;
        }

        private void OnMouseDown()
        {
            GetAllCells();
        }

        private void GetAllCells()
        {
            GetAquariumCells?.Invoke(currentCells[NumCell], CountCells);
            if (CountCells != 0)
            {
                particleSystem.Play();
                StartCoroutine(WaitParticleSystem(0.3f));
            }
            DisplayCount.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = CountCells.ToString();
            DisplayCount.GetComponent<Animator>().SetBool("On", true);
            StartCoroutine(waitDisplayCount());

            Spawners[currentCells[NumCell]].PutIngradient(CountCells);

            CountCells = 0;
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
    [Serializable]
    public class Ground
    {
        public float timeExist;

    }
}
