using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string IngredientName = "None";

    private Vector3 startPos;

    private Vector3 mousePosition;
    public bool OnDrag = false;
    public bool InTableMix = false;

    public GameObject spawner;

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {

        mousePosition = Input.mousePosition - GetMousePos();
        OnDrag = true;

    }
    private void OnMouseDrag()
    {
        if (OnDrag)
        {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition).y, transform.position.z);
        }

    }
    private void OnMouseUp()
    {
        if (OnDrag && !GetComponent<Ingredient>().InTableMix)
        {
            if (spawner != null)
            {
                spawner.GetComponent<Spawner>().count++;
            }
            else
            {
                StoreManager.Instance.AddIngridient(IngredientName);
            }
            Destroy(gameObject);
        }
    }
}
