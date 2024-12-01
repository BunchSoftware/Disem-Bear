using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private FileProducts fileProducts;
    private List<ProductGUI> productsGUI = new List<ProductGUI>();
    private PlayerShop playerShop;
    public Action<Product> OnBuyProduct;

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    private void Start()
    {
        playerShop = FindFirstObjectByType<PlayerShop>();
        List<Product> products = new List<Product>();

        for (int i = 0; i < fileProducts.products.Count; i++)
        {
            Product product = new Product()
            {
               money = fileProducts.products[i].money,
               countProduct = fileProducts.products[i].countProduct,
               rewardText = fileProducts.products[i].rewardText,
               header = fileProducts.products[i].header,
               avatar = fileProducts.products[i].avatar,
            };
            products.Add(product);
        }

        for (int i = 0; i < products.Count; i++)
        {
            prefab.name = $"Product {i}";
            Instantiate(prefab, transform);
        }

        for (int i = 0; i < products.Count; i++)
        {
            ProductGUI productGUI;

            if (gameObject.transform.GetChild(i).TryGetComponent<ProductGUI>(out productGUI))
            {
                productGUI.Init(
                (product) =>
                {
                    if (Buy(product) && product.countProduct - 1 >= 0)
                    {
                        product.countProduct--;
                        productGUI.UpdateData(product);
                    }
                }, 
                () =>
                {
                    Remove(productGUI);
                }, products[i]);
                productsGUI.Add(productGUI);

            };
        }
        Sort();
    }

    private void Sort()
    {
        for (int j = 0; j < productsGUI.Count; j++)
        {
            if (productsGUI[j].GetProduct().countProduct == -1)
                transform.GetChild(j).SetAsFirstSibling();
        }

        for (var i = 1; i < productsGUI.Count; i++)
        {
            for (var j = 0; j < productsGUI.Count - i; j++)
            {
                if (productsGUI[j].GetProduct().countProduct != -1)
                {
                    var temp = productsGUI[j];
                    productsGUI[j] = productsGUI[j + 1];
                    productsGUI[j + 1] = temp;
                }
            }
        }
    }

    private bool Buy(Product product)
    {
        if (playerShop.playerShopInfo.money - product.money >= 0)
        {
            playerShop.playerShopInfo.money -= product.money;
            OnBuyProduct?.Invoke(product);

            return true;
        }
        return false;
    }

    private void Remove(ProductGUI productGUI)
    {
        productsGUI.Remove(productGUI);
        Destroy(productGUI.gameObject);
        Sort();
    }
}
