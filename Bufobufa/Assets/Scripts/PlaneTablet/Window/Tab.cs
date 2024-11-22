using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tab : MonoBehaviour
{
    public int indexTab;
    private bool isSelected = false;

    public void SelectTab(bool isSelected)
    {
        this.isSelected = isSelected;

        if (isSelected)
           gameObject.SetActive(true);
        else
           gameObject.SetActive(false);
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }
}
