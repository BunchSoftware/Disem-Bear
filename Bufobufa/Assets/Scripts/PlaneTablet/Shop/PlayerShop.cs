using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShop : MonoBehaviour
{
    public PlayerShopInfo playerShopInfo;

    private SaveManager saveManager;

    private void Start()
    {
        saveManager = new SaveManager();
    }
}
