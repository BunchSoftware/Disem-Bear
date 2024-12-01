using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ThingsInTableMix : MonoBehaviour
{

    public List<GameObject> IngredientsIn = new();
    public List<string> ingredients = new();
    public List<Recipe> Recipes = new();
    public bool MixTableOn = false;
    public GameObject currentPrinterObject;

    private InfoInstObj currentCreatObj;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Ingredient>())
        {
            IngredientsIn.Add(collision.gameObject);
            collision.gameObject.GetComponent<Ingredient>().InTableMix = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<Ingredient>())
        {
            IngredientsIn.Remove(collision.gameObject);
            collision.gameObject.GetComponent<Ingredient>().InTableMix = false;
        }
    }

    public void MixIngredients()
    {
        transform.parent.GetComponent<OpenObject>().ArgumentsNotQuit += 1;
        bool NotExistReceip = true;
        ingredients.Clear();
        for (int i = 0; i < IngredientsIn.Count; i++)
        {
            ingredients.Add(IngredientsIn[i].GetComponent<Ingredient>().IngredientName);
        }
        ingredients.Sort();
        for (int i = 0; i < Recipes.Count; i++)
        {
            List<string> IngredientStrings = new();
            for (int k = 0; k < Recipes[i].IngredientsForRecipe.Count; k++)
            {
                IngredientStrings.Add(Recipes[i].IngredientsForRecipe[k].GetComponent<Ingredient>().IngredientName);
            }
            IngredientStrings.Sort();
            if (ingredients.SequenceEqual(IngredientStrings))
            {
                if (Recipes[i].OutPut != null)
                {
                    NotExistReceip = false;
                    GameObject tempObj = IngredientsIn[0];
                    for (int j = IngredientsIn.Count - 1; j >= 0; j--)
                    {
                        IngredientsIn[j].GetComponent<AnimDeleteIngredients>().DeleteIngredient();
                        //Destroy(IngredientsIn[j]);
                    }
                    IngredientsIn.Clear();

                    currentCreatObj = new();
                    currentCreatObj.obj = Recipes[i].OutPut;
                    currentCreatObj.pos = transform.position;
                    currentCreatObj.rot = tempObj.transform.rotation;
                    currentCreatObj.par = transform.parent;

                    StartCoroutine(WaitAnimDelete(1f));

                    break;
                }
            }
        }
        if (NotExistReceip) transform.parent.GetComponent<OpenObject>().ArgumentsNotQuit -= 1;
    }
    public void ClearIngredients()
    {
        for (int j = IngredientsIn.Count - 1; j >= 0; j--)
        {
            IngredientsIn[j].GetComponent<Ingredient>().ResetIngredient();
            Destroy(IngredientsIn[j]);
        }
        IngredientsIn = new();
    }
    [Serializable]
    public class Recipe
    {
        public List<GameObject> IngredientsForRecipe;
        public GameObject OutPut;
    }

    IEnumerator WaitAnimDelete(float f)
    {
        yield return new WaitForSeconds(f);
        GameObject obj = Instantiate(currentCreatObj.obj, currentCreatObj.pos, currentCreatObj.rot, currentCreatObj.par);
        obj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        obj.GetComponent<AnimDeleteIngredients>().CreateIngredient();
        StartCoroutine(WaitAnimCreate(1f, obj));

    }
    IEnumerator WaitAnimCreate(float f, GameObject obj)
    {
        yield return new WaitForSeconds(f);
        if (obj.GetComponent<PrinterObjectInfo>())
        {
            MixTableOn = false;
            currentPrinterObject = obj;
            obj.transform.rotation = currentCreatObj.obj.transform.rotation;
            obj.transform.localScale = new Vector3(obj.transform.localScale.x / transform.parent.localScale.x, obj.transform.localScale.y / transform.parent.localScale.y, obj.transform.localScale.z / transform.parent.localScale.z);
        }
        transform.parent.GetComponent<OpenObject>().ArgumentsNotQuit -= 1;
    }
    private class InfoInstObj
    {
        public GameObject obj;
        public Vector3 pos;
        public Quaternion rot;
        public Transform par;
    }
}
