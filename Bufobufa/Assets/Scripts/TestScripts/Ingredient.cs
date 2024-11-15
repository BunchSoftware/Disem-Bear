using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string IngredientName = "None";

    private Vector3 startPos;

    private Vector3 mousePosition;
    public int count = 0;
    private bool OnDrag = false;
    public bool InTableMix = false;

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
    private void OnMouseDown()
    {
        count--;
        if (count != 0)
        {
            Instantiate(gameObject, transform.parent);
        }
        OnDrag = true;
        mousePosition = Input.mousePosition - GetMousePos();
        transform.GetChild(0).GetComponent<Animator>().SetBool("ShadeOff", false);
    }
    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }
    private void OnMouseUp()
    {
        if (!InTableMix)
        {
            count++;
            transform.position = startPos;
        }
    }
    private void OnMouseEnter()
    {
        if (!OnDrag)
        {
            transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshPro>().text = count.ToString();
            transform.GetChild(0).GetComponent<Animator>().SetBool("ShadeOff", true);
        }
    }
    private void OnMouseExit()
    {
        OnDrag = false;
        transform.GetChild(0).GetComponent<Animator>().SetBool("ShadeOff", false);
    }
    private void Start()
    {
        startPos = transform.position;
        if (count == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
