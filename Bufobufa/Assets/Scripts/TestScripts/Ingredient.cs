using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string IngredientName = "None";

    public bool InTableMix = false;

    public GameObject spawner;

    private void OnMouseUp()
    {
        if (!InTableMix)
        {
            ResetIngredient();
            Destroy(gameObject);
        }
    }
    public void ResetIngredient()
    {
        if (spawner != null)
        {
            spawner.GetComponent<Spawner>().count++;
        }
        else
        {
            StoreManager.Instance.AddIngridient(IngredientName);
        }
    }
}
