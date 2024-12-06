using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAquariumMaterial : MonoBehaviour
{
    private PostOfficeTube PostTube;

    [SerializeField] private GameObject blueMaterial;
    [SerializeField] private GameObject purpleMaterial;
    [SerializeField] private GameObject orangeMaterial;

    private bool ExistPackage = false;

    private void Start()
    {
        PostTube = GameObject.Find("PostOfficeTube").GetComponent<PostOfficeTube>();
    }

    public void PurpleButtonMaterial()
    {
        GetMaterial("Purple");
    }
    public void BlueButtonMaterial()
    {
        GetMaterial("Blue");
    }
    public void OrangeButtonMaterial()
    {
        GetMaterial("Orange");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && ExistPackage)
        {
            StartCoroutine(WaitExitUI(0.5f));
        }
    }

    public string GetMaterial(string nameMaterial)
    {
        if (!PostTube.ItemExist && PostTube.NotObjectDown)
        {
            if (nameMaterial == "Blue")
            {
                PostTube.currentObj = blueMaterial;
                PostTube.ItemExist = true;
                ExistPackage = true;
                return "OK";
            }
            else if (nameMaterial == "Purple")
            {
                PostTube.currentObj = purpleMaterial;
                PostTube.ItemExist = true;
                ExistPackage = true;
                return "OK";
            }
            else if (nameMaterial == "Orange")
            {
                PostTube.currentObj = orangeMaterial;
                PostTube.ItemExist = true;
                ExistPackage = true;
                return "OK";
            }
            return "WrongName";
        }
        else
        {
            return "PostOfficeBusy";
        }
    }

    IEnumerator WaitExitUI(float f)
    {
        yield return new WaitForSeconds(f);
        PostTube.ObjectFall();
    }
}
