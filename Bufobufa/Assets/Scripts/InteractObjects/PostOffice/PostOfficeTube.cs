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
    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
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
                currentFallObj.GetComponent<MoveAnimation>().StartMove();
                ItemExist = false;
            }
        }
    }
    private void Update()
    {
        
    }
}
