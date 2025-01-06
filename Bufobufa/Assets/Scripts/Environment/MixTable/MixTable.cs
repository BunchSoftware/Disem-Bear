using External.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Environment.LMixTable
{
    public class MixTable : MonoBehaviour
    {
        public SaveManager saveManager;
        public List<Ingredient> TypesIngredients = new();

        public static MixTable Instance;


        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            //if (saveManager.filePlayer.JSONPlayer.resources.ingradientSaves == null || saveManager.filePlayer.JSONPlayer.resources.ingradientSaves.Count == 0)
            //{
            //    saveManager.filePlayer.JSONPlayer.resources.ingradientSaves = new List<IngradientSave> { };

            //    for (int i = 0; i < TypesIngredients.Count; i++)
            //    {
            //        saveManager.filePlayer.JSONPlayer.resources.ingradientSaves.Add(new IngradientSave()
            //        {
            //            typeIngradient = TypesIngredients[i].name,
            //            countIngradient = TypesIngredients[i].Spawner.GetComponent<Spawner>().count
            //        });
            //        saveManager.UpdatePlayerFile();
            //    }
            //}

            //for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.ingradientSaves.Count; i++)
            //{
            //    for (int j = 0; j < TypesIngredients.Count; j++)
            //    {
            //        if (saveManager.filePlayer.JSONPlayer.resources.ingradientSaves[i].typeIngradient == TypesIngredients[i].name)
            //        {
            //            TypesIngredients[i].Spawner.GetComponent<Spawner>().count = saveManager.filePlayer.JSONPlayer.resources.ingradientSaves[i].countIngradient;
            //        }
            //    }
            //}
        }

        public void AddIngridient(string nameIngridient)
        {
            for (int i = 0; i < TypesIngredients.Count; i++)
            {
                if (TypesIngredients[i].name == nameIngridient)
                {
                    TypesIngredients[i].Spawner.GetComponent<Spawner>().count++;

                    for (int j = 0; j < TypesIngredients.Count; j++)
                    {
                        if (saveManager.filePlayer.JSONPlayer.resources.ingradientSaves[i].typeIngradient == TypesIngredients[i].name)
                        {
                            saveManager.filePlayer.JSONPlayer.resources.ingradientSaves[i].countIngradient = TypesIngredients[i].Spawner.GetComponent<Spawner>().count;
                        }
                    }

                    saveManager.UpdatePlayerFile();
                }
            }
        }

        [Serializable]
        public class Ingredient
        {
            public string name;
            public GameObject Spawner;
        }
    }
}
