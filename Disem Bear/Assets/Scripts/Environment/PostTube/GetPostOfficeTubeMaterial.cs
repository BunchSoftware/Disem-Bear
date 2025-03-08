using Game.Environment.LPostTube;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment
{
    [Serializable]
    public class TypePostOfficeTubeMaterial
    {
        public string typeMaterial;
        public GameObject material;
    }

    public class GetPostOfficeTubeMaterial : MonoBehaviour
    {
        [SerializeField] private PostTube PostTube;
        [SerializeField] private List<TypePostOfficeTubeMaterial> typePostOfficeTubeMaterials;
        public UnityEvent<TypePostOfficeTubeMaterial> OnGetPostOfficeTubeMaterial;

        private GameObject getMaterial;

        private void Start()
        {
            //GetMaterial("DocumentBluePackage");
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) && getMaterial != null)
            {
                StartCoroutine(WaitExitUI(0.5f));
            }
        }

        public void GetMaterial(string typeMaterial)
        {
            for (int i = 0; i < typePostOfficeTubeMaterials.Count; i++)
            {
                if (typePostOfficeTubeMaterials[i].typeMaterial == typeMaterial)
                {
                    getMaterial = typePostOfficeTubeMaterials[i].material;
                    OnGetPostOfficeTubeMaterial?.Invoke(typePostOfficeTubeMaterials[i]);

                    return;
                }
            }
        }

        IEnumerator WaitExitUI(float f)
        {
            yield return new WaitForSeconds(f);
            PostTube.ObjectFall(getMaterial);
            getMaterial = null;
        }
    }
}
