using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.Environment.LMixTable
{
    [Serializable]
    public class Ingradient
    {
        public string typeIngradient;
        public int countIngradient;
    }

    public class IngradientSpawner : MonoBehaviour
    {
        [SerializeField] private Ingradient ingradient;

        private bool OnDrag = false;
        [SerializeField] private GameObject spriteIngredient;
        [SerializeField] private GameObject DisplayCount;
        private GameObject IngredientObj;

        private Vector3 GetMousePos()
        {
            return Camera.main.WorldToScreenPoint(IngredientObj.transform.position);
        }
        private void OnMouseDown()
        {
            if (ingradient.countIngradient != 0)
            {
                ingradient.countIngradient--;
                //IngredientObj = Instantiate(Ingredient, transform.position, transform.rotation, transform.parent.parent);
                OnDrag = true;
            }
        }
        private void OnMouseUp()
        {
            //if (OnDrag && !IngredientObj.GetComponent<MoveObjectMouse>().InTableMix)
            //{
            //    ingradient.countIngradient++;
            //    Destroy(IngredientObj);
            //}
        }
        private void OnMouseEnter()
        {
            if (ingradient.countIngradient != 0)
            {
                //DisplayCount.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshPro>().text = ingradient.countIngradient.ToString();
                //DisplayCount.GetComponent<Animator>().SetBool("On", true);
            }
        }
        private void OnMouseExit()
        {
            //DisplayCount.GetComponent<Animator>().SetBool("On", false);
        }
        private void Update()
        {
            //if (ingradient.countIngradient == 0)
            //{
            //    spriteIngredient.SetActive(false);
            //}
            //else
            //{
            //    spriteIngredient.SetActive(true);
            //}
        }

        public Ingradient GetIngradient()
        {
            return ingradient;
        }
    }
}
