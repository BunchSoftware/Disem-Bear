using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment.LMixTable
{
    public class AnimDeleteIngredients : MonoBehaviour
    {
        private GameObject MixTable;
        private float TimeAnimation = 1f;
        private bool DeleteOn = false;
        private bool CreateOn = false;
        private float timer = 0f;

        private Vector3 StartCoord;
        private Vector3 EndCoord;
        private Vector3 StartRotation;
        private Vector3 EndRotation;
        private Vector3 StartScale;
        private Vector3 EndScale;

        public void DeleteIngredient()
        {
            DeleteOn = true;

            StartCoord = transform.position;
            EndCoord = MixTable.transform.position;
            StartRotation = new Vector3(0, 0, 0);
            EndRotation = new Vector3(0, 0, 1440f);
            StartScale = transform.localScale;
            EndScale = new Vector3(0.01f, 0.01f, 0.01f);

            timer = 0f;
        }

        public void CreateIngredient(Vector3 lcS)
        {
            CreateOn = true;


            StartRotation = new Vector3(0, 0, 0);
            EndRotation = new Vector3(0, 0, 1440f);
            StartScale = transform.localScale;
            EndScale = lcS;

            timer = 0f;
        }
        private void Start()
        {
            MixTable = GameObject.Find("MixTable");
        }
        private void Update()
        {
            if (DeleteOn)
            {
                if (timer <= TimeAnimation)
                {
                    timer += Time.deltaTime;
                    transform.position = Vector3.Lerp(StartCoord, EndCoord, timer / TimeAnimation);
                    transform.localEulerAngles = Vector3.Lerp(StartRotation, EndRotation, timer / TimeAnimation);
                    transform.localScale = Vector3.Lerp(StartScale, EndScale, timer / TimeAnimation);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            if (CreateOn)
            {
                if (timer <= TimeAnimation)
                {
                    timer += Time.deltaTime;
                    transform.localEulerAngles = Vector3.Lerp(StartRotation, EndRotation, timer / TimeAnimation);
                    transform.localScale = Vector3.Lerp(StartScale, EndScale, timer / TimeAnimation);
                }
            }
        }
    }
}
