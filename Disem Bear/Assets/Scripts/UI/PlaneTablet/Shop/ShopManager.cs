using External.Storage;
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
        [SerializeField] private SaveManager saveManager;
        private List<ProductGUI> productsGUI = new List<ProductGUI>();
        public Action<Product> OnBuyProduct;

        public void Init(SaveManager saveManager)
        {
            this.saveManager = saveManager;
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
                            reward = fileProducts.products[i].reward,
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
                    reward = saveManager.fileShop.JSONShop.resources.productSaves[i].reward,
                    countPriceChange = saveManager.fileShop.JSONShop.resources.productSaves[i].countPriceChange,
                    header = fileProducts.products[i].header,
                    avatarPriceChange = fileProducts.products[i].avatarPriceChange,
                };
                if (saveManager.fileShop.JSONShop.resources.productSaves[i].reward.countReward != 0)
                    products.Add(product);
            }

            for (int i = 0; i < products.Count; i++)
            {
                prefab.name = $"Product {i}";
                GameObject.Instantiate(prefab, content.transform);
            }

            for (int i = 0; i < products.Count; i++)
            {
                ProductGUI productGUI;

                if (content.transform.GetChild(i).TryGetComponent<ProductGUI>(out productGUI))
                {
                    productGUI.Init(
                    (product) =>
                    {
                        if (Buy(product) && product.reward.countReward - 1 >= 0)
                        {
                            product.reward.countReward--;
                            saveManager.fileShop.JSONShop.resources.productSaves[product.indexProduct].reward = product.reward;
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
                if (productsGUI[j].GetProduct().reward.countReward == -1)
                    content.transform.GetChild(j).SetAsFirstSibling();
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
                                if (saveManager.filePlayer.JSONPlayer.resources.products[j].typeProduct == product.reward.typeReward)
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
                                typeProduct = product.reward.typeReward,
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
