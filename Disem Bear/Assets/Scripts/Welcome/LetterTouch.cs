using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LetterTouch : MonoBehaviour, IPointerDownHandler
{
    private Animator animator;
    [SerializeField] private GameObject Flare;
    private List<string> bools = new List<string>() { "First", "Second", "Third" };
    private int index = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        animator.SetBool(bools[index], true);
        index++;
        index %= bools.Count;
    }

    public void FlareOn()
    {
        Flare.SetActive(true);
        StartCoroutine(OffLetter());
    }

    IEnumerator OffLetter()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }
}
