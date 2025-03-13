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
        private List<ProductGUI> productsGUI = new List<ProductGUI>();
        public Action<Product> OnBuyProduct;

        public void Init()
        {
            List<Product> products = fileProducts.products;


            //if (SaveManager.fileShop.JSONShop != null)
            //{
            //    if (SaveManager.fileShop.JSONShop.resources.productSaves == null || SaveManager.fileShop.JSONShop.resources.productSaves.Count == 0)
            //    {
            //        List<ProductSave> productSaves = new List<ProductSave>();
            //        for (int i = 0; i < fileProducts.products.Count; i++)
            //        {
            //            productSaves.Add(new ProductSave()
            //            {
            //                reward = fileProducts.products[i].reward,
            //                countPriceChange = fileProducts.products[i].countPriceChange,
            //                typePriceChangeProduct = fileProducts.products[i].typePriceChangeProduct,
            //            });
            //        }
            //        SaveManager.fileShop.JSONShop.resources.productSaves = productSaves;
            //    }
            //}

            //for (int i = 0; i < fileProducts.products.Count; i++)
            //{
            //    Product product = new Product()
            //    {
            //        indexProduct = i,
            //        typePriceChangeProduct = SaveManager.fileShop.JSONShop.resources.productSaves[i].typePriceChangeProduct,
            //        reward = SaveManager.fileShop.JSONShop.resources.productSaves[i].reward,
            //        countPriceChange = SaveManager.fileShop.JSONShop.resources.productSaves[i].countPriceChange,
            //        header = fileProducts.products[i].header,
            //        avatarPriceChange = fileProducts.products[i].avatarPriceChange,
            //    };
            //    if (SaveManager.fileShop.JSONShop.resources.productSaves[i].reward.countReward != 0)
            //        products.Add(product);
            //}

            for (int i = 0; i < products.Count; i++)
            {
                prefab.name = $"Product {i}";
                ProductGUI productGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ProductGUI>();
                productsGUI.Add(productGUI);
            }

            for (int i = 0; i < content.transform.childCount; i++)
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
                            SaveManager.fileShop.JSONShop.resources.productSaves[product.indexProduct].reward = product.reward;
                            SaveManager.fileShop.JSONShop.resources.productSaves[product.indexProduct].countPriceChange = product.price;

                            productGUI.UpdateData(product);
                        }
                    },
                    () =>
                    {
                        Remove(productGUI);
                    }, products[i]);
                };
            }
            Sort();
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
            if (SaveManager.filePlayer.JSONPlayer.resources.products != null)
            {
                for (int i = 0; i < SaveManager.filePlayer.JSONPlayer.resources.products.Count; i++)
                {
                    if (SaveManager.filePlayer.JSONPlayer.resources.products[i].typeProduct == product.typePriceProduct)
                    {
                        if (SaveManager.filePlayer.JSONPlayer.resources.products[i].countProduct - product.price >= 0)
                        {
                            SaveManager.filePlayer.JSONPlayer.resources.products[i].countProduct -= product.price;
                            for (int j = 0; j < SaveManager.filePlayer.JSONPlayer.resources.products.Count; j++)
                            {
                                if (SaveManager.filePlayer.JSONPlayer.resources.products[j].typeProduct == product.reward.typeReward)
                                {
                                    SaveManager.filePlayer.JSONPlayer.resources.products[j].countProduct += 1;
                                    GiveProduct(product);
                                    OnBuyProduct?.Invoke(product);

                                    return true;
                                }
                            }

                            SaveManager.filePlayer.JSONPlayer.resources.products.Add(new ProductData()
                            {
                                countProduct = 1,
                                typeProduct = product.reward.typeReward,
                            });

                            GiveProduct(product);

                            Debug.Log($"{product.reward.typeReward} был куплен");

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
