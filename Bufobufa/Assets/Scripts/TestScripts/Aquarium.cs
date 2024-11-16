using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aquarium : MonoBehaviour
{
    public string NameIngredient = "None";
    private GameObject DisplayCount;
    private bool InTrigger = false;
    public bool NormalTemperature = false;
    public bool NormalGround = false;
    public bool OnAquarium = false;
    public float NormalTimeCell = 3f;
    private float TimeCell = 666f;
    private float timerCell = 0f;
    public int CountCells = 0;

    private void OnTriggerEnter(Collider other)
    {
        InTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    {
        InTrigger = false;
    }
    private void Start()
    {
        TimeCell = NormalTimeCell;
        DisplayCount = transform.Find("DisplayCount").gameObject;
    }
    private void Update()
    {
        if (InTrigger && CountCells > 0) DisplayCount.GetComponent<Animator>().SetBool("On", true);
        else DisplayCount.GetComponent<Animator>().SetBool("On", false);
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

        if (InTrigger && Input.GetKeyDown(KeyCode.E))
        {
            for (int i = 0; i < CountCells; i++)
            {
                StoreManager.Instance.AddIngridient(NameIngredient);
            }
            CountCells = 0;
        }
    }
}
