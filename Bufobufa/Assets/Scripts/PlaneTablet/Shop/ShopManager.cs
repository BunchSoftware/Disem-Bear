using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class TypeMachineDispensingProduct
{
    public string typeMachineDispensingProduct;
    public UnityEvent<string> OnGetProduct;
}
public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private FileProducts fileProducts;
    [SerializeField] private List<TypeMachineDispensingProduct> typeGiveProducts;
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
            if (saveManager.fileShop.JSONShop.resources.productSaves == null || saveManager.fileShop.JSONShop.resources.productSaves.Count == 0)
            {
                List<ProductSave> productSaves = new List<ProductSave>();
                for (int i = 0; i < fileProducts.products.Count; i++)
                {
                    productSaves.Add(new ProductSave()
                    {
                        countChangeProduct = fileProducts.products[i].countChangeProduct,
                        typeChangeProduct = fileProducts.products[i].typeChangeProduct,
                        typeMachineDispensingProduct = fileProducts.products[i].typeMachineDispensingProduct,
                        countPriceChange = fileProducts.products[i].countPriceChange,
                        typePriceChangeProduct = fileProducts.products[i].typePriceChangeProduct,
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
                typePriceChangeProduct = saveManager.fileShop.JSONShop.resources.productSaves[i].typePriceChangeProduct,
                typeChangeProduct = saveManager.fileShop.JSONShop.resources.productSaves[i].typeChangeProduct,
                typeMachineDispensingProduct = saveManager.fileShop.JSONShop.resources.productSaves[i].typeMachineDispensingProduct,
                countChangeProduct = saveManager.fileShop.JSONShop.resources.productSaves[i].countChangeProduct,
                countPriceChange = saveManager.fileShop.JSONShop.resources.productSaves[i].countPriceChange,
                header = fileProducts.products[i].header,
                avatarPriceChange = fileProducts.products[i].avatarPriceChange,
                avatarChange = fileProducts.products[i].avatarChange,
            };
            if (saveManager.fileShop.JSONShop.resources.productSaves[i].countChangeProduct != 0)
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
                    if (Buy(product) && product.countChangeProduct - 1 >= 0)
                    {
                        saveManager.fileShop.JSONShop.resources.productSaves[product.indexProduct].countChangeProduct = product.countChangeProduct;
                        saveManager.fileShop.JSONShop.resources.productSaves[product.indexProduct].countPriceChange = product.countPriceChange;
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
            if (productsGUI[j].GetProduct().countChangeProduct == -1)
                transform.GetChild(j).SetAsFirstSibling();
        }

        for (var i = 1; i < productsGUI.Count; i++)
        {
            for (var j = 0; j < productsGUI.Count - i; j++)
            {
                if (productsGUI[j].GetProduct().countChangeProduct != -1)
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
        if (saveManager.filePlayer.JSONPlayer.resources.products != null)
        {
            for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.products.Count; i++)
            {
                if (saveManager.filePlayer.JSONPlayer.resources.products[i].typeProduct == product.typePriceChangeProduct)
                {
                    if (saveManager.filePlayer.JSONPlayer.resources.products[i].countProduct - product.countPriceChange >= 0)
                    {
                        saveManager.filePlayer.JSONPlayer.resources.products[i].countProduct -= product.countPriceChange;
                        for (int j = 0; j < saveManager.filePlayer.JSONPlayer.resources.products.Count; j++)
                        {
                            if (saveManager.filePlayer.JSONPlayer.resources.products[j].typeProduct == product.typeChangeProduct)
                            {
                                saveManager.filePlayer.JSONPlayer.resources.products[j].countProduct += 1;
                                saveManager.UpdatePlayerFile();
                                GiveProduct(product);
                                OnBuyProduct?.Invoke(product);

                                return true;
                            }
                        }

                        saveManager.filePlayer.JSONPlayer.resources.products.Add(new SaveTypeProduct()
                        {
                            countProduct = 1,
                            typeProduct = product.typeChangeProduct
                        });

                        GiveProduct(product);

                        saveManager.UpdatePlayerFile();
                        OnBuyProduct?.Invoke(product);

                        return true;
                    }   
                }
            }
        }
        return false;
    }

    private void GiveProduct(Product product)
    {
        if (typeGiveProducts != null)
        {
            for (int i = 0; i < typeGiveProducts.Count; i++)
            {
                if (typeGiveProducts[i].typeMachineDispensingProduct == product.typeMachineDispensingProduct)
                {
                    typeGiveProducts[i].OnGetProduct.Invoke(product.typeChangeProduct);
                }
            }
        }
    }

    private void Remove(ProductGUI productGUI)
    {
        productsGUI.Remove(productGUI);
        Destroy(productGUI.gameObject);
        Sort();
    }
}
