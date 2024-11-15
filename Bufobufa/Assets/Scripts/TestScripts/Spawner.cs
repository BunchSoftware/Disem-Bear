using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public string IngredientName = "None";

    private Vector3 startPos;

    private Vector3 mousePosition;
    public int count = 0;
    private bool OnDrag = false;
    public bool InTableMix = false;
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
            IngredientObj.GetComponent<Ingredient>().IngredientName = IngredientName;
            IngredientObj.GetComponent<Ingredient>().spawner = gameObject;
            IngredientObj.GetComponent<SpriteRenderer>().sprite = spriteIngredient.GetComponent<SpriteRenderer>().sprite;
            IngredientObj.AddComponent<BoxCollider2D>();
            OnDrag = true;
            mousePosition = Input.mousePosition - GetMousePos();
        }
    }
    private void OnMouseDrag()
    {
        if (OnDrag)
        {
            IngredientObj.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition).y, transform.position.z);
        }
    }
    private void OnMouseUp()
    {
        if (OnDrag && !IngredientObj.GetComponent<Ingredient>().InTableMix)
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
        spriteIngredient = transform.Find("Sprite").gameObject;
        DisplayCount = transform.Find("DisplayCount").gameObject;
        startPos = transform.position;
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
