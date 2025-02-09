using Game.Environment.Printer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Game.Environment.LMixTable
{
    public class ThingsInTableMix : MonoBehaviour
    {
        public List<GameObject> IngredientsIn = new();
        public List<string> ingredients = new();
        public List<Recipe> Recipes = new();
        public bool MixTableOn = false;
        public GameObject currentPrinterObject;
        public bool CreateObject = false;
        public bool IsPrinterObject = false;

        private InfoInstObj currentCreatObj;

        private ParticleSystem ParticleSystem;

        private void Start()
        {
            ParticleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.GetComponent<IngradientSpawner>())
            {
                IngredientsIn.Add(collision.gameObject);
                //collision.gameObject.GetComponent<MoveObjectMouse>().InTableMix = true;
            }
        }
        private void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject.GetComponent<IngradientSpawner>())
            {
                IngredientsIn.Remove(collision.gameObject);
                //collision.gameObject.GetComponent<MoveObjectMouse>().InTableMix = false;
            }
        }

        public void MixIngredients()
        {
            if (MixTableOn)
            {
                //transform.parent.GetComponent<OpenObject>().ArgumentsNotQuit += 1;
                bool NotExistReceip = true;
                ingredients.Clear();
                for (int i = 0; i < IngredientsIn.Count; i++)
                {
                    ingredients.Add(IngredientsIn[i].GetComponent<IngradientSpawner>().GetIngradient().typeIngradient);
                }
                ingredients.Sort();
                for (int i = 0; i < Recipes.Count; i++)
                {
                    List<string> IngredientStrings = new();
                    for (int k = 0; k < Recipes[i].IngredientsForRecipe.Count; k++)
                    {
                        IngredientStrings.Add(Recipes[i].IngredientsForRecipe[k].GetComponent<IngradientSpawner>().GetIngradient().typeIngradient);
                    }
                    IngredientStrings.Sort();
                    if (ingredients.SequenceEqual(IngredientStrings))
                    {
                        if (Recipes[i].OutPut != null)
                        {
                            CreateObject = true;
                            NotExistReceip = false;
                            GameObject tempObj = IngredientsIn[0];
                            var temp = IngredientsIn[0].transform.position;
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
                            ParticleSystem.Play();
                            StartCoroutine(WaitAnimDelete(1f));

                            break;
                        }
                    }
                }
                //if (NotExistReceip) transform.parent.GetComponent<OpenObject>().ArgumentsNotQuit -= 1;
            }
        }
        public void ClearIngredients()
        {
            for (int j = IngredientsIn.Count - 1; j >= 0; j--)
            {
                //IngredientsIn[j].GetComponent<MoveObjectMouse>().ResetIngredient();
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
            obj.transform.localRotation = currentCreatObj.rot;
            var lcS = obj.transform.localScale;
            obj.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);

            obj.GetComponent<AnimDeleteIngredients>().CreateIngredient(lcS);
            StartCoroutine(WaitAnimCreate(1f, obj));

        }
        IEnumerator WaitAnimCreate(float f, GameObject obj)
        {
            yield return new WaitForSeconds(f);
            ParticleSystem.Stop();
            if (obj.GetComponent<PrinterObjectInfo>())
            {
                MixTableOn = false;
                currentPrinterObject = obj;
                IsPrinterObject = true;
            }
            else
            {
                IsPrinterObject = false;
            }
            CreateObject = false;
            //transform.parent.GetComponent<OpenObject>().ArgumentsNotQuit -= 1;
        }
        private class InfoInstObj
        {
            public GameObject obj;
            public Vector3 pos;
            public Quaternion rot;
            public Transform par;
        }
    }
}
