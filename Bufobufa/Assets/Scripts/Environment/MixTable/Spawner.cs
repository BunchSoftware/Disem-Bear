using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private string IngredientName = "None";

    private Vector3 mousePosition;
    public int count = 0;
    private bool OnDrag = false;
    private GameObject spriteIngredient;
    private GameObject DisplayCount;

    [SerializeField] private GameObject Ingredient;
    private GameObject IngredientObj;

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(IngredientObj.transform.position);
    }
    private void OnMouseDown()
    {
        if (count != 0)
        {
            count--;
            IngredientObj = Instantiate(Ingredient, transform.position, transform.rotation, transform.parent.parent);
            OnDrag = true;
            mousePosition = Input.mousePosition - GetMousePos();
        }
    }
    private void OnMouseUp()
    {
        if (OnDrag && !IngredientObj.GetComponent<MoveObjectMouse>().InTableMix)
        {
            count++;
            Destroy(IngredientObj);
        }
    }
    private void OnMouseEnter()
    {
        if (count != 0)
        {
            DisplayCount.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshPro>().text = count.ToString();
            DisplayCount.GetComponent<Animator>().SetBool("On", true);
        }
    }
    private void OnMouseExit()
    {
        DisplayCount.GetComponent<Animator>().SetBool("On", false);
    }
    private void Start()
    {
        IngredientName = Ingredient.GetComponent<Ingredient>().IngredientName;
        spriteIngredient = transform.Find("Sprite").gameObject;
        DisplayCount = transform.Find("DisplayCount").gameObject;
    }
    private void Update()
    {
        if (count == 0)
        {
            spriteIngredient.SetActive(false);
        }
        else
        {
            spriteIngredient.SetActive(true);
        }
    }
}
