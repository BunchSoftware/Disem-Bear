using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostOfficeTube : MonoBehaviour
{
    public bool ItemExist = false;
    public GameObject currentObj;
    public static PostOfficeTube Instance;
    public Vector3 TubePosition;
    private GameObject currentFallObj;
    private GameObject Player;
    private Vector3 PointObject;
    private ParticleSystem ParticleSystem;
    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        PointObject = transform.Find("PointObject").transform.position;
        ParticleSystem = transform.Find("Particle System").GetComponent<ParticleSystem>();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (ItemExist)
        {
            if (currentObj)
            {
                currentFallObj = Instantiate(currentObj, TubePosition, currentObj.transform.rotation);
                currentFallObj.GetComponent<MoveAnimation>().needPosition = true;
                currentFallObj.GetComponent<MoveAnimation>().TimeAnimation = 1f;
                currentFallObj.GetComponent<MoveAnimation>().startCoords = PointObject;
                currentFallObj.GetComponent<MoveAnimation>().StartMove();
                StartCoroutine(ParticleFall(currentFallObj.GetComponent<MoveAnimation>().TimeAnimation));
                ItemExist = false;
            }
        }
    }
    IEnumerator ParticleFall(float t)
    {
        yield return new WaitForSeconds(t);
        ParticleSystem.Play();
        yield return new WaitForSeconds(0.2f);
        ParticleSystem.Stop();
    }
}
