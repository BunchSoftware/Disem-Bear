using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PutSeal : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject Medved;
    [SerializeField] private GameObject Fader;
    public void OnPointerDown(PointerEventData eventData)
    {
        Medved.SetActive(true);
        StartCoroutine(NextScene());
    }

    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(2f);
        Fader.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(3);
    }
}
