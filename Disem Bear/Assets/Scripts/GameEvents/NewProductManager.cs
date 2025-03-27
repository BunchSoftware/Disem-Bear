using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.PlaneTablet.Exercise;
using UI.PlaneTablet.Shop;
using UnityEngine;

public class NewProductManager : MonoBehaviour
{
    [SerializeField] private UIGameRoot uIGameRoot;
    private ShopManager shopManager;
    [SerializeField] private List<ProductNew> newProducts = new();
    public List<EventNewProducts> eventsNewProduct = new();


    public void Init(EventsManager eventsManager)
    {
        shopManager = uIGameRoot.GetShopManager();
        eventsManager.onEventBegin.AddListener((ev) =>
        {
            if (ev.textJson.indexProduct != -1)
            {
                EventNewProducts eventNewProducts = new()
                {
                    nameEvent = ev.name,
                    index = ev.textJson.indexProduct,
                    startTime = ServerToReal(ev.start_date_time),
                    duration = ev.duration_in_minutes
                };
                AddTempProduct(eventNewProducts);
            }
        });
        eventsManager.onEventEnd.AddListener((ev) =>
        {
            if (ev.textJson.indexProduct != -1)
            {
                EventNewProducts eventNewProducts = new()
                {
                    nameEvent = ev.name,
                    index = ev.textJson.indexProduct,
                    startTime = ServerToReal(ev.start_date_time),
                    duration = ev.duration_in_minutes
                };
                RemoveTempProduct(eventNewProducts);
            }
        });
    }

    public void OnUpdate(float deltaTime)
    {

    }


    public void AddTempProduct(EventNewProducts productEvent)
    {

        newProducts[productEvent.index].productGUI = shopManager.AddProduct(newProducts[productEvent.index].product);
    }

    public void RemoveTempProduct(EventNewProducts productEvent)
    {
        shopManager.Remove(newProducts[productEvent.index].productGUI);
    }


    private DateTime ServerToReal(string serverTime)
    {
        DateTime dt = DateTime.Parse(serverTime);
        return dt;
    }
}

public class EventNewProducts
{
    public string nameEvent;
    public int index = 0;
    public DateTime startTime;
    public float duration = 1f;
}

[System.Serializable]
public class ProductNew
{
    public Product product;
    public ProductGUI productGUI;
}


