using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temperature : MonoBehaviour
{
    [SerializeField] List<Sprite> States = new();
    public int numState = 5;
    private float timer = 0;
    public float TimeLessOneLevel = 5f;

    private void OnMouseDown()
    {
        numState = Mathf.Min(numState + 1, States.Count - 1);
        GetComponent<SpriteRenderer>().sprite = States[numState];
        if (numState > 3)
        {
            transform.parent.gameObject.GetComponent<Aquarium>().NormalTemperature = true;
        }
        else
        {
            transform.parent.gameObject.GetComponent<Aquarium>().NormalTemperature = false;
        }
    }
    private void Start()
    {

        numState = Mathf.Min(numState, States.Count - 1);
        GetComponent<SpriteRenderer>().sprite = States[numState];
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > TimeLessOneLevel)
        {
            timer = 0f;
            numState = Mathf.Max(0, numState - 1);
            GetComponent<SpriteRenderer>().sprite = States[numState];
            if (numState > 3)
            {
                transform.parent.gameObject.GetComponent<Aquarium>().NormalTemperature = true;
            }
            else
            {
                transform.parent.gameObject.GetComponent<Aquarium>().NormalTemperature = false;
            }
        }
    }
}
