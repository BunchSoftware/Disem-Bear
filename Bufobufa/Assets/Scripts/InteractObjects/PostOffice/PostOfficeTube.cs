using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostOfficeTube : MonoBehaviour
{
    public bool ItemExist = false;
    public GameObject prefabObject;
    public static PostOfficeTube Instance;
    public Vector3 TubePosition;
    private GameObject currentFallObj;
    private GameObject Player;
    private Vector3 PointObject;
    private ParticleSystem ParticleSystem;
    public bool NotObjectDown = true;
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
        if (other.tag == "Player")
        {
            ObjectFall();
        }
    }
    public void ObjectFall()
    {
        if (ItemExist && NotObjectDown)
        {
            if (prefabObject)
            {
                currentFallObj = Instantiate(prefabObject, TubePosition, prefabObject.transform.rotation);
                currentFallObj.GetComponent<GetItemFromTable>().isTube = true;
                currentFallObj.GetComponent<MoveAnimation>().needPosition = true;
                currentFallObj.GetComponent<MoveAnimation>().TimeAnimation = 1f;
                currentFallObj.GetComponent<MoveAnimation>().startCoords = PointObject;
                currentFallObj.GetComponent<MoveAnimation>().StartMove();
                StartCoroutine(ParticleFall(currentFallObj.GetComponent<MoveAnimation>().TimeAnimation));
                ItemExist = false;
                NotObjectDown = false;
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
