using External.Storage;
using Game.Environment.Item;
using Game.Environment.LPostTube;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        [SerializeField] private PostBox postBox;
        [SerializeField] private PostTube postTube;
        [SerializeField] private FileProducts fileProducts;
        [SerializeField] private List<TypeMachineDispensingProduct> typeGiveProducts;

        private ToastManager toastManager;
        private MonoBehaviour context;
        private List<ProductGUI> productsGUIs = new List<ProductGUI>();
        public Action<Product> OnBuyProduct;

        public void Init(MonoBehaviour context, ToastManager toastManager)
        {
            this.toastManager = toastManager;
            this.context = context;
            List<Product> products = new List<Product>();

            if(SaveManager.fileShop.JSONShop.resources.productSaves != null)
            {
                for (int i = 0; i < SaveManager.fileShop.JSONShop.resources.productSaves.Count; i++)
                {
                    Product product = FindProductToFileProducts(SaveManager.fileShop.JSONShop.resources.productSaves[i].typeReward);
                    if (product != null)
                    {
                        product.reward.countReward = SaveManager.fileShop.JSONShop.resources.productSaves[i].countReward;
                        product.isVisible = SaveManager.fileShop.JSONShop.resources.productSaves[i].isVisible;

                        if(product.reward.countReward != 0)
                        {
                            prefab.name = $"Product {i}";
                            ProductGUI productGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ProductGUI>();
                            productsGUIs.Add(productGUI);
                            products.Add(product);
                        }
                    }
                }
            }

            for (int i = 0; i < productsGUIs.Count; i++)
            {
                ProductGUI productGUI = productsGUIs[i];
                productGUI.Init(
                (product) =>
                {
                    if (postBox.ItemInbox() == false && postTube.IsItemFlies() == false)
                    {
                        if (Buy(product))
                        {
                            if (product.reward.countReward != -1)
                                product.reward.countReward--;
                            productGUI.UpdateData(product);
                            toastManager.ShowToast("Товар был куплен, посмотрите в доставке");
                        }
                        else
                        {
                            toastManager.ShowToast("Недостаточно средств для обмена !");
                        }
                    }
                    else
                    {
                        toastManager.ShowToast("Доставка занята другим предметом, заберите его, а потом закажите новый");
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
            for (int j = 0; j < productsGUIs.Count; j++)
            {
                if (productsGUIs[j].GetProduct().reward.countReward == -1)
                {
                    content.transform.GetChild(j).SetAsFirstSibling();
                }
            }

            for (var i = 1; i < productsGUIs.Count; i++)
            {
                for (var j = 0; j < productsGUIs.Count - i; j++)
                {
                    if (productsGUIs[j].GetProduct().reward.countReward != -1)
                    {
                        var temp = productsGUIs[j];
                        productsGUIs[j] = productsGUIs[j + 1];
                        productsGUIs[j + 1] = temp;
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
                        Debug.Log(product.reward.typeReward);
                        typeGiveProducts[i].OnGetReward?.Invoke(product.reward);
                    }
                }
            }
        }

        public void SetVisibleProduct(string typeReward,  bool isVisible)
        {
            for (int i = 0; i < productsGUIs.Count; i++)
            {
                Product product = productsGUIs[i].GetProduct();
                if (product.reward.typeReward == typeReward)
                {
                    product.isVisible = isVisible;
                    productsGUIs[i].UpdateData(product);

                    for (int j = 0; j < SaveManager.fileShop.JSONShop.resources.productSaves.Count; j++)
                    {
                        if (SaveManager.fileShop.JSONShop.resources.productSaves[j].typeReward == typeReward)
                            SaveManager.fileShop.JSONShop.resources.productSaves[j].isVisible = isVisible;
                    }

                    return;
                }    
            }
        }

        public void SetVisibleProductInDuration(string typeReward, bool isVisible)
        {
            for (int i = 0; i < productsGUIs.Count; i++)
            {
                Product product = productsGUIs[i].GetProduct();
                if (product.reward.typeReward == typeReward)
                {
                    context.StartCoroutine(IVisibleProductTime(productsGUIs[i], product.durationOfAppearance, isVisible));
                    return;
                }
            }
        }

        IEnumerator IVisibleProductTime(ProductGUI productGUI, float time, bool isVisible)
        {
            yield return new WaitForSeconds(time);
            Product product = productGUI.GetProduct();
            product.isVisible = isVisible;
            productGUI.UpdateData(product);

            for (int j = 0; j < SaveManager.fileShop.JSONShop.resources.productSaves.Count; j++)
            {
                if (SaveManager.fileShop.JSONShop.resources.productSaves[j].typeReward == product.reward.typeReward)
                    SaveManager.fileShop.JSONShop.resources.productSaves[j].isVisible = isVisible;
            }
        }

        private void Remove(ProductGUI productGUI)
        {
            productsGUIs.Remove(productGUI);
            GameObject.Destroy(productGUI.gameObject);
            Sort();
        }
    }
}
