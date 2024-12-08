using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TypePostOfficeTubeMaterial
{
    public string typeMaterial;
    public GameObject material;
}

public class GetPostOfficeTubeMaterial : MonoBehaviour
{
    private PostOfficeTube PostTube;
    [SerializeField] private List<TypePostOfficeTubeMaterial> typePostOfficeTubeMaterials;
    public UnityEvent<TypePostOfficeTubeMaterial> OnGetPostOfficeTubeMaterial;

    private bool ExistPackage = false;

    private void Start()
    {
        PostTube = GameObject.Find("PostOfficeTube").GetComponent<PostOfficeTube>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && ExistPackage)
        {
            StartCoroutine(WaitExitUI(0.5f));
        }
    }

    public void GetMaterial(string typeMaterial)
    {
        if (!PostTube.ItemExist && PostTube.NotObjectDown)
        {
            for (int i = 0; i < typePostOfficeTubeMaterials.Count; i++)
            {
                if (typePostOfficeTubeMaterials[i].typeMaterial == typeMaterial)
                {
                    PostTube.prefabObject = typePostOfficeTubeMaterials[i].material;
                    PostTube.ItemExist = true;
                    ExistPackage = true;

                    OnGetPostOfficeTubeMaterial?.Invoke(typePostOfficeTubeMaterials[i]);

                    return;
                }
            }
        }
    }

    IEnumerator WaitExitUI(float f)
    {
        yield return new WaitForSeconds(f);
        PostTube.ObjectFall();
    }
}
