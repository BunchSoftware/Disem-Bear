using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private SaveManager saveManager;    
    public bool PlayerInSomething = false;
    public bool PlayerPickSometing = false;

    private void Awake()
    {
        saveManager = FindFirstObjectByType<SaveManager>();
    }

    public GameObject currentPickObject;
}
