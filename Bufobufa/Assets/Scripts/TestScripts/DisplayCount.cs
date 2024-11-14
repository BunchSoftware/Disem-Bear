using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayCount : MonoBehaviour
{
    public int count = 0;
    private void Update()
    {
        if (gameObject == RayCastCamera.Instance.currentObj)
        {
            transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshPro>().text = count.ToString();
            transform.GetChild(0).GetComponent<Animator>().SetBool("ShadeOff", true);
        }
        else
        {
            transform.GetChild(0).GetComponent<Animator>().SetBool("ShadeOff", false);
        }
    }
}
