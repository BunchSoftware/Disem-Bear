using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private FileProducts fileProducts;
    [SerializeField] private SaveManager saveManager;
    private List<ProductGUI> productsGUI = new List<ProductGUI>();
    public Action<Product> OnBuyProduct;

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    private void Start()
    {
        List<Product> products = new List<Product>();


        if (saveManager.fileShop.JSONShop != null)
        {
            if (saveManager.fileShop.JSONShop.resources.productSaves.Count == 0)
            {
                List<ProductSave> productSaves = new List<ProductSave>();
                for (int i = 0; i < fileProducts.products.Count; i++)
                {
                    productSaves.Add(new ProductSave()
                    {
                        countProduct = fileProducts.products[i].countProduct,
                        money = fileProducts.products[i].money,
                    });
                }
                saveManager.fileShop.JSONShop.resources.productSaves = productSaves;
            }
        }

        for (int i = 0; i < fileProducts.products.Count; i++)
        {
            Product product = new Product()
            {
               indexProduct = i,
               money = saveManager.fileShop.JSONShop.resources.productSaves[i].money,
               countProduct = saveManager.fileShop.JSONShop.resources.productSaves[i].countProduct,
               rewardText = fileProducts.products[i].rewardText,
               header = fileProducts.products[i].header,
               avatar = fileProducts.products[i].avatar,
            };
            if (saveManager.fileShop.JSONShop.resources.productSaves[i].countProduct != 0)
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

                        saveManager.fileShop.JSONShop.resources.productSaves[product.indexProduct].money = product.money;
                        saveManager.fileShop.JSONShop.resources.productSaves[product.indexProduct].countProduct = product.countProduct;
                        saveManager.UpdateShopFile();

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
        if (saveManager.filePlayer.JSONPlayer.resources.money - product.money >= 0)
        {
            saveManager.filePlayer.JSONPlayer.resources.money -= product.money;
            saveManager.UpdatePlayerFile();
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
