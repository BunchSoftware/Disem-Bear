using External.Storage;
using Game.Environment.Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Environment.LMixTable
{
    public class MixTable : MonoBehaviour
    {
        [SerializeField] private List<IngradientSpawner> ingradientSpawners;

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        [SerializeField] private SaveManager saveManager;

        public void Init(SaveManager saveManager)
        {
            this.saveManager = saveManager;
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

        public void AddIngradient(Ingradient ingradient)
        {
            for (int i = 0; i < ingradientSpawners.Count; i++)
            {
                Ingradient ingradientSpawner = ingradientSpawners[i].GetIngradient();
                if (ingradientSpawner.typeIngradient == ingradient.typeIngradient)
                {
                    ingradientSpawner.countIngradient += ingradient.countIngradient;
                }
            }
        }

        public void ReduceIngradient(Ingradient ingradient)
        {
            for(int i = 0; i < ingradientSpawners.Count; i++)
            {
                Ingradient ingradientSpawner = ingradientSpawners[i].GetIngradient();
                if (ingradientSpawner.typeIngradient == ingradient.typeIngradient)
                {
                    ingradientSpawner.countIngradient -= ingradient.countIngradient;
                }
            }
        }
    }
}
