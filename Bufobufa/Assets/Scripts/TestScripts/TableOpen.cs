using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOpen : MonoBehaviour
{
    public GameObject DisplayCount;
    public bool InTrigger = false;
    private bool TableIsOpen = false;

    [SerializeField] GameObject MixButton;
    [SerializeField] GameObject Damper;

    private void Start()
    {
        DisplayCount = transform.Find("DisplayCount").gameObject;
    }
    public void OnTrigEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = true;
            DisplayCount.GetComponent<Animator>().SetBool("On", true);
        }
    }
    public void OnTrigExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InTrigger = false;
            DisplayCount.GetComponent<Animator>().SetBool("On", false);
        }
    }
    private void Update()
    {
        if (InTrigger && Input.GetKeyDown(KeyCode.E) && !TableIsOpen){
            //GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Animator>().SetBool("On", true);
            MixButton.SetActive(true);
            TableIsOpen = true;
            Damper.SetActive(false);
        }
        else if (TableIsOpen && Input.GetKeyDown(KeyCode.E))
        {
            GetComponent<Animator>().SetBool("On", false);
            StartCoroutine(CoroutineForAnim());
            MixButton.SetActive(false);
            TableIsOpen = false;
            Damper.SetActive(true);
        }
    }
    private IEnumerator CoroutineForAnim()
    {
        yield return new WaitForSeconds(0.5f);
        //GetComponent<BoxCollider>().isTrigger = false;
    }
}
