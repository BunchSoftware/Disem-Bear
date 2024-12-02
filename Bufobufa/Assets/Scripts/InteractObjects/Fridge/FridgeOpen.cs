using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeOpen : MonoBehaviour
{
    private bool OneTap = true;
    private GameObject FrontFridge;
    public List<GameObject> Magnets = new List<GameObject>();
    private void Start()
    {
        FrontFridge = transform.Find("FrontFridge").gameObject;
    }

    private void Update()
    {
        if (GetComponent<OpenObject>().ObjectIsOpen && OneTap)
        {
            OneTap = false;
            FrontFridge.SetActive(true);
            for (int i = 0; i < Magnets.Count; i++)
            {
                Magnets[i].GetComponent<BoxCollider>().enabled = true;
                Magnets[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else if (!GetComponent<OpenObject>().ObjectIsOpen && !OneTap)
        {
            OneTap = true;
            FrontFridge.SetActive(false);
            for (int i = 0; i < Magnets.Count; i++)
            {
                Magnets[i].GetComponent<BoxCollider>().enabled = false;
                Magnets[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void ChangeMouseTrigger()
    {
        for (int i = 0; i < Magnets.Count; i++)
        {
            if (Magnets[i].GetComponent<MagnetMouseMove>().OnDrag)
            {
                Magnets[i].GetComponent<MouseTrigger>().enabled = true;
            }
            else
            {
                Magnets[i].GetComponent<MouseTrigger>().enabled = false;
            }
        }
    }

    public void OnMouseTrigger()
    {
        for (int i = 0; i < Magnets.Count; i++)
        {
            Magnets[i].GetComponent<MouseTrigger>().enabled = true;
        }
    }
}
