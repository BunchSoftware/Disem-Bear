using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingsInTableMix : MonoBehaviour
{
    public List<string> IngredientsIn = new();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Ingredient>())
        {
            IngredientsIn.Add(other.gameObject.GetComponent<Ingredient>().IngredientName);
            other.gameObject.GetComponent<Ingredient>().InTableMix = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Ingredient>())
        {
            IngredientsIn.Remove(other.gameObject.GetComponent<Ingredient>().IngredientName);
            other.gameObject.GetComponent<Ingredient>().InTableMix = false;
        }
    }
}
