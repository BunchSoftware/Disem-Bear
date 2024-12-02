using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string IngredientName = "None";
    public float TimeInAquarium = 1f;

    public bool InTableMix = false;


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
        StoreManager.Instance.AddIngridient(IngredientName);
    }
}
