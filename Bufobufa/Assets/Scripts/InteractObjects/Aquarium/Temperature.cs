using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temperature : MonoBehaviour
{
    [SerializeField] List<Sprite> States = new();
    public int numState = 5;
    private float timer = 0;
    public float TimeLessOneLevel = 5f;

    [SerializeField] DialogManager Dialog; //”ƒ¿À»“‹
    public string CodeWord = ""; //”ƒ¿À»“‹
    [SerializeField] GameObject OffStrela;
    [SerializeField] GameObject OnStrela;
    private void OnMouseDown()
    {
        Dialog.RunConditionSkip(CodeWord); //”ƒ¿À»“‹
        OffStrela.SetActive(false); //”ƒ¿À»“‹
        OnStrela.SetActive(true); //”ƒ¿À»“‹

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
        Dialog = GameObject.Find("DialogManager").GetComponent<DialogManager>(); //”ƒ¿À»“‹

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
