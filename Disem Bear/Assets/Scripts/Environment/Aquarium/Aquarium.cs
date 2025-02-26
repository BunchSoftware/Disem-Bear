using External.Storage;
using Game.Environment.LMixTable;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Environment.Aquarium
{
    public class Aquarium : MonoBehaviour
    {
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
        private Dictionary <string, IngradientSpawner> Spawners  = new();
        private int NumCell = 0;
        private SpriteRenderer ChoiceCellSprite;
        private SpriteRenderer spriteRenderer;
        private ParticleSystem particleSystem;
        private GameObject DisplayCount;
        [SerializeField] private ChangeCell buttonLeft;
        [SerializeField] private ChangeCell buttonRight;

        private int CountCells = 0;



        public void Init(SaveManager saveManager, Player player)
        {
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

            UpdateMaterial(materialForAquarium);

            buttonLeft.Init(this);
            buttonRight.Init(this);
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
                    if (timeMaterial <= 0f)
                    {
                        spriteRenderer.sprite = phasesAquariums[colorMaterial].NullFaseDirty;
                    }
                    else
                    {
                        spriteRenderer.sprite = phasesAquariums[colorMaterial].NullFase;
                    }
                }
                else if (CountCells < 4)
                {
                    if (timeMaterial <= 0f)
                    {
                        spriteRenderer.sprite = phasesAquariums[colorMaterial].FirstFaseDirty;
                    }
                    else
                    {
                        spriteRenderer.sprite = phasesAquariums[colorMaterial].FirstFase;
                    }
                }
                else if (CountCells < 9)
                {
                    if (timeMaterial <= 0f)
                    {
                        spriteRenderer.sprite = phasesAquariums[colorMaterial].SecondFaseDirty;
                    }
                    else
                    {
                        spriteRenderer.sprite = phasesAquariums[colorMaterial].SecondFase;
                    }
                }
                else if (CountCells < 15)
                {
                    if (timeMaterial <= 0f)
                    {
                        spriteRenderer.sprite = phasesAquariums[colorMaterial].ThirdFaseDirty;
                    }
                    else
                    {
                        spriteRenderer.sprite = phasesAquariums[colorMaterial].ThirdFase;
                    }
                }
            }
            if (timeMaterial > 0f) spendTimeCreateCell += Time.deltaTime;
            if (spendTimeCreateCell >= timeCells[currentCells[NumCell]])
            {
                if (CountCells < 15)
                    CountCells++;
                spendTimeCreateCell = 0;
            }
        }

        private void UpdateMaterial(MaterialForAquarium materialForAquarium)
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

        IEnumerator WaitParticleSystem(float f)
        {
            yield return new WaitForSeconds(f);
            particleSystem.Stop();
        }

        IEnumerator waitDisplayCount()
        {
            yield return new WaitForSeconds(2);
            DisplayCount.GetComponent<Animator>().SetBool("On", false);
        }
        //public void ChangeCell(int ch)
        //{
        //    GetAllCells();
        //    NumCell = (NumCell + ch + CellsList.Count) % CellsList.Count;
        //    //NameIngredient = CellsList[NumCell].GetComponent<IngradientItem>().typeIngradient;
        //    ChoiceCellSprite.sprite = CellsList[NumCell].GetComponent<SpriteRenderer>().sprite;
        //    //NormalTimeCell = CellsList[NumCell].GetComponent<IngradientItem>().TimeInAquarium;
        //    timerCell = 0f;
        //}
        //private void OnMouseDown()
        //{
        //    GetAllCells();
        //}

        //private void GetAllCells()
        //{
        //    if (CountCells != 0)
        //    {
        //        ParticleSystemm.Play();
        //        StartCoroutine(WaitParticleSystem(0.3f));
        //    }
        //    DisplayCount.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = CountCells.ToString();
        //    DisplayCount.GetComponent<Animator>().SetBool("On", true);
        //    StartCoroutine(waitDisplayCount());
        //    for (int i = 0; i < CountCells; i++)
        //    {
        //        //MixTable.Instance.AddIngridient(NameIngredient);
        //    }
        //    CountCells = 0;
        //}

        //IEnumerator waitDisplayCount()
        //{
        //    yield return new WaitForSeconds(2);
        //    DisplayCount.GetComponent<Animator>().SetBool("On", false);
        //}
        //private void Start()
        //{
        //    ParticleSystemm = transform.Find("Particle System").GetComponent<ParticleSystem>();
        //    spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        //    ChoiceCellSprite = transform.Find("ChoiceCell").GetComponent<SpriteRenderer>();
        //    if (CellsList.Count > 0)
        //    {
        //        //NameIngredient = CellsList[NumCell].GetComponent<IngradientItem>().typeIngradient;
        //    }
        //    ChoiceCellSprite.sprite = CellsList[NumCell].GetComponent<SpriteRenderer>().sprite;
        //    //NormalTimeCell = CellsList[NumCell].GetComponent<IngradientItem>().TimeInAquarium;

        //    TimeCell = NormalTimeCell;
        //    DisplayCount = transform.Find("DisplayCount").gameObject;
        //}
        //private void Update()
        //{
        //    if (TimeWaterSpend > 0f)
        //    {
        //        TimeWaterSpend -= Time.deltaTime;
        //    }
        //    if (CountCells == 0 && OnAquarium)
        //    {
        //        if (NameMaterial != "Classic" && TimeWaterSpend <= 0f)
        //        {
        //            spriteRenderer.sprite = phasesAquarium.NullFaseDirty;
        //        }
        //        else
        //        {
        //            spriteRenderer.sprite = phasesAquarium.NullFase;
        //        }
        //    }
        //    else if (CountCells < 4 && OnAquarium)
        //    {
        //        if (NameMaterial != "Classic" && TimeWaterSpend <= 0f)
        //        {
        //            spriteRenderer.sprite = phasesAquarium.FirstFaseDirty;
        //        }
        //        else
        //        {
        //            spriteRenderer.sprite = phasesAquarium.FirstFase;
        //        }
        //    }
        //    else if (CountCells < 9 && OnAquarium)
        //    {
        //        if (NameMaterial != "Classic" && TimeWaterSpend <= 0f)
        //        {
        //            spriteRenderer.sprite = phasesAquarium.SecondFaseDirty;
        //        }
        //        else
        //        {
        //            spriteRenderer.sprite = phasesAquarium.SecondFase;
        //        }
        //    }
        //    else if (CountCells < 15 && OnAquarium)
        //    {
        //        if (NameMaterial != "Classic" && TimeWaterSpend <= 0f)
        //        {
        //            spriteRenderer.sprite = phasesAquarium.ThirdFaseDirty;
        //        }
        //        else
        //        {
        //            spriteRenderer.sprite = phasesAquarium.ThirdFase;
        //        }
        //    }
        //    if (NormalTemperature) TimeCell = NormalTimeCell;
        //    else TimeCell = NormalTimeCell * 2;
        //    if (TimeWaterSpend > 0f || NameMaterial == "Classic") timerCell += Time.deltaTime;
        //    if (timerCell >= TimeCell)
        //    {
        //        if (CountCells < 15)
        //            CountCells++;
        //        timerCell = 0;
        //    }
        //}
        //IEnumerator WaitParticleSystem(float f)
        //{
        //    yield return new WaitForSeconds(f);
        //    ParticleSystemm.Stop();
        //}
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
