using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Aquarium : MonoBehaviour
{
    public string NameIngredient = "None";
    private GameObject DisplayCount;
    public bool NormalTemperature = false;
    public bool NormalGround = false;
    public bool OnAquarium = false;
    public float NormalTimeCell = 3f;
    private float TimeCell = 666f;
    private float timerCell = 0f;
    public int CountCells = 0;

    [SerializeField] private Sprite NullFase;
    [SerializeField] private Sprite FirstFase;
    [SerializeField] private Sprite SecondFase;
    [SerializeField] private Sprite ThirdFase;

    private void OnMouseDown()
    {
        DisplayCount.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = CountCells.ToString();
        DisplayCount.GetComponent<Animator>().SetBool("On", true);
        StartCoroutine(waitDisplayCount());
        for (int i = 0; i < CountCells; i++)
        {
            StoreManager.Instance.AddIngridient(NameIngredient);
        }
        CountCells = 0;
    }
    IEnumerator waitDisplayCount()
    {
        yield return new WaitForSeconds(2);
        DisplayCount.GetComponent<Animator>().SetBool("On", false);
    }
    private void Start()
    {

        TimeCell = NormalTimeCell;
        DisplayCount = transform.Find("DisplayCount").gameObject;
    }
    private void Update()
    {
        if (NormalTemperature || NormalGround) OnAquarium = true;
        else OnAquarium = false;
        if (OnAquarium) timerCell += Time.deltaTime;
        if (timerCell >= TimeCell)
        {
            CountCells++;
            timerCell = 0;
        }
        if (NormalTemperature && NormalGround) TimeCell = NormalTimeCell;
        else if (NormalTemperature || NormalGround) TimeCell = NormalTimeCell * 2;

        if (CountCells == 0)
        {
            GetComponent<SpriteRenderer>().sprite = NullFase;
        }
        else if (CountCells < 4)
        {
            GetComponent<SpriteRenderer>().sprite = FirstFase;
        }
        else if (CountCells < 9)
        {
            GetComponent<SpriteRenderer>().sprite = SecondFase;
        }
        else if (CountCells < 15)
        {
            GetComponent<SpriteRenderer>().sprite = ThirdFase;
        }
    }
}
