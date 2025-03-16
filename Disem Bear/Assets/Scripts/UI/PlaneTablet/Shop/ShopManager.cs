using External.Storage;
using Game.Environment.Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.PlaneTablet.Shop
{
    [Serializable]
    public class TypeMachineDispensingProduct
    {
        public string typeMachineDispensingProduct;
        public UnityEvent<Reward> OnGetReward;
    }

    [Serializable]
    public class ShopManager
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject content;
        [SerializeField] private FileProducts fileProducts;
        [SerializeField] private List<TypeMachineDispensingProduct> typeGiveProducts;
        private List<ProductGUI> productsGUI = new List<ProductGUI>();
        public Action<Product> OnBuyProduct;

        public void Init()
        {
            List<Product> products = new List<Product>();

            if(SaveManager.fileShop.JSONShop.resources.productSaves != null)
            {
                for (int i = 0; i < SaveManager.fileShop.JSONShop.resources.productSaves.Count; i++)
                {
                    Product product = FindProductToFileProducts(SaveManager.fileShop.JSONShop.resources.productSaves[i].typeReward);
                    product.reward.countReward = SaveManager.fileShop.JSONShop.resources.productSaves[i].countReward;
                    if (product != null && product.reward.countReward != 0)
                    {
                        prefab.name = $"Product {i}";
                        ProductGUI productGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ProductGUI>();
                        productsGUI.Add(productGUI);
                        products.Add(product);
                    }
                }
            }

            for (int i = 0; i < productsGUI.Count; i++)
            {
                ProductGUI productGUI = productsGUI[i];
                productGUI.Init(
                (product) =>
                {
                    if (Buy(product) && product.reward.countReward - 1 >= 0)
                    {
                        product.reward.countReward--;
                        productGUI.UpdateData(product);
                    }
                },
                () =>
                {
                    Remove(productGUI);
                }, products[i]);
            }
            Sort();
        }

        private Product FindProductToFileProducts(string typeReward)
        {
            for (int i = 0; i < fileProducts.products.Count; i++)
            {
                if (fileProducts.products[i].reward.typeReward == typeReward && typeReward.Length >= 1)
                    return fileProducts.products[i];
            }

            return null;
        }

        private void Sort()
        {
            for (int j = 0; j < productsGUI.Count; j++)
            {
                if (productsGUI[j].GetProduct().reward.countReward == -1)
                {
                    content.transform.GetChild(j).SetAsFirstSibling();
                }
            }

            for (var i = 1; i < productsGUI.Count; i++)
            {
                for (var j = 0; j < productsGUI.Count - i; j++)
                {
                    if (productsGUI[j].GetProduct().reward.countReward != -1)
                    {
                        var temp = productsGUI[j];
                        productsGUI[j] = productsGUI[j + 1];
                        productsGUI[j + 1] = temp;
                    }
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        }

        private bool Buy(Product product)
        {
            if (SaveManager.fileShop.JSONShop.resources.productSaves != null)
            {
                for (int i = 0; i < SaveManager.fileShop.JSONShop.resources.productSaves.Count; i++)
                {
                    if (SaveManager.fileShop.JSONShop.resources.productSaves[i].typeReward == product.reward.typeReward)
                    {
                        for (int j = 0; j < SaveManager.filePlayer.JSONPlayer.resources.ingradients.Count; j++)
                        {
                            if (SaveManager.filePlayer.JSONPlayer.resources.ingradients[j].typeIngradient == product.typePriceProduct &&
                               SaveManager.filePlayer.JSONPlayer.resources.ingradients[j].countIngradient - product.price >= 0)
                            {
                                SaveManager.filePlayer.JSONPlayer.resources.ingradients[j].countIngradient -= product.price;
                                SaveManager.fileShop.JSONShop.resources.productSaves[i].countReward -= 1;
                                GiveProduct(product);
                                OnBuyProduct?.Invoke(product);

                                Debug.Log($"{product.reward.typeReward} был куплен");
                                return true;
                            }
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
                    if (typeGiveProducts[i].typeMachineDispensingProduct == product.reward.typeMachineDispensingReward)
                    {
                        typeGiveProducts[i].OnGetReward?.Invoke(product.reward);
                    }
                }
            }
        }

        private void Remove(ProductGUI productGUI)
        {
            productsGUI.Remove(productGUI);
            GameObject.Destroy(productGUI.gameObject);
            Sort();
        }
    }
}
