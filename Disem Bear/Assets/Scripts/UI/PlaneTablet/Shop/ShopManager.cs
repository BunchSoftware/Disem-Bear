using External.Storage;
using Game.Environment.LPostTube;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet.Exercise;
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
        public UnityEvent<ExerciseItem> OnGetExerciseItem;
    }

    [Serializable]
    public class DispensingTask
    {
        public TypeMachineDispensingProduct typeMachineDispensingProduct;
        public Reward reward;
        public ExerciseItem exerciseItem;
    }

    [Serializable]
    public class ShopManager
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject content;
        [SerializeField] private PostBox postBox;
        [SerializeField] private PostTube postTube;
        [SerializeField] private ProductDatabase fileProducts;
        [SerializeField] private List<TypeMachineDispensingProduct> typeGiveProducts;

        private ToastManager toastManager;
        private MonoBehaviour context;
        private List<ProductGUI> productsGUIs = new List<ProductGUI>();
        public Action<Product> OnBuyProduct;

        private List<DispensingTask> dispensingTasks = new List<DispensingTask>();
        private const int MaxObjectFall = 3;
        public void Init(MonoBehaviour context, TV tv, ToastManager toastManager)
        {
            this.toastManager = toastManager;
            this.context = context;
            List<Product> products = new List<Product>();

            if (SaveManager.shopDatabase.JSONShop.resources.productSaves != null)
            {
                for (int i = 0; i < SaveManager.shopDatabase.JSONShop.resources.productSaves.Count; i++)
                {
                    Product product = FindProductToFileProducts(SaveManager.shopDatabase.JSONShop.resources.productSaves[i].typeReward);
                    if (product != null)
                    {
                        product.reward.countReward = SaveManager.shopDatabase.JSONShop.resources.productSaves[i].countReward;
                        product.isVisible = SaveManager.shopDatabase.JSONShop.resources.productSaves[i].isVisible;

                        if (product.reward.countReward != 0)
                        {     
                            ProductGUI productGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ProductGUI>();
                            productGUI.name = $"Product {i}";
                            productsGUIs.Add(productGUI);
                            products.Add(product);
                        }
                    }
                }
            }

            SaveManager.UpdateShopDatabase();

            for (int i = 0; i < productsGUIs.Count; i++)
            {
                ProductGUI productGUI = productsGUIs[i];
                productGUI.Init(
                (product) =>
                {
                    if (postBox.ItemInBox() == false && postTube.IsItemFlies() == false)
                    {
                        if (dispensingTasks.Count < MaxObjectFall)
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
                            toastManager.ShowToast("Достигнуто максимальное количество предметов на выдачу");
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

            tv.OnTVClose.AddListener(() =>
            {
                if (dispensingTasks.Count > 0)
                {
                    Debug.Log(dispensingTasks.Count);
                    for (int i = 0; i < dispensingTasks.Count; i++)
                    {
                        DispensingTask dispensingTask = dispensingTasks[i];
                        dispensingTask.typeMachineDispensingProduct.OnGetReward?.Invoke(dispensingTask.reward);
                    }
                    dispensingTasks.Clear();
                }
            });

            Debug.Log("ShopManager: Успешно иницилизирован");
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
            for (int i = productsGUIs.Count - 1; i > -1; i--)
            {
                if (productsGUIs[i].GetProduct().isImporttant)
                {
                    content.transform.GetChild(i).SetSiblingIndex(0);
                }
            }
            productsGUIs.Clear();
            for (int i = 0; i < content.transform.childCount; i++)
            {
                productsGUIs.Add(content.transform.GetChild(i).GetComponent<ProductGUI>());
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        }

        private bool Buy(Product product)
        {
                if (SaveManager.shopDatabase.JSONShop.resources.productSaves != null)
                {
                    for (int i = 0; i < SaveManager.shopDatabase.JSONShop.resources.productSaves.Count; i++)
                    {
                        if (SaveManager.shopDatabase.JSONShop.resources.productSaves[i].typeReward == product.reward.typeReward)
                        {
                            for (int j = 0; j < SaveManager.playerDatabase.JSONPlayer.resources.ingradients.Count; j++)
                            {
                                if (SaveManager.playerDatabase.JSONPlayer.resources.ingradients[j].typeIngradient == product.typePriceProduct &&
                                   SaveManager.playerDatabase.JSONPlayer.resources.ingradients[j].countIngradient - product.price >= 0)
                                {
                                    SaveManager.playerDatabase.JSONPlayer.resources.ingradients[j].countIngradient -= product.price;
                                    SaveManager.shopDatabase.JSONShop.resources.productSaves[i].countReward -= 1;
                                    GiveProduct(product);
                                    OnBuyProduct?.Invoke(product);

                                    Debug.Log($"{product.reward.typeReward} был куплен");
                                    SaveManager.UpdateShopDatabase();
                                    SaveManager.UpdatePlayerDatabase();
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
                        DispensingTask dispensingTask = new DispensingTask();
                        dispensingTask.typeMachineDispensingProduct = typeGiveProducts[i];
                        dispensingTask.reward = product.reward;

                        if (dispensingTasks.Count < MaxObjectFall)
                        {
                            dispensingTasks.Add(dispensingTask);
                        }
                        else
                        {
                            toastManager.ShowToast("Достигнуто максимальное количество предметов на выдачу");
                        }
                    }
                }
            }
        }

        public void SetVisibleProduct(string typeReward, bool isVisible)
        {
            for (int i = 0; i < productsGUIs.Count; i++)
            {
                Product product = productsGUIs[i].GetProduct();
                if (product.reward.typeReward == typeReward)
                {
                    product.isVisible = isVisible;
                    productsGUIs[i].UpdateData(product);

                    for (int j = 0; j < SaveManager.shopDatabase.JSONShop.resources.productSaves.Count; j++)
                    {
                        if (SaveManager.shopDatabase.JSONShop.resources.productSaves[j].typeReward == typeReward)
                            SaveManager.shopDatabase.JSONShop.resources.productSaves[j].isVisible = isVisible;
                    }

                    SaveManager.UpdateShopDatabase();

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

        private IEnumerator IVisibleProductTime(ProductGUI productGUI, float time, bool isVisible)
        {
            yield return new WaitForSeconds(time);
            Product product = productGUI.GetProduct();
            product.isVisible = isVisible;
            productGUI.UpdateData(product);

            for (int j = 0; j < SaveManager.shopDatabase.JSONShop.resources.productSaves.Count; j++)
            {
                if (SaveManager.shopDatabase.JSONShop.resources.productSaves[j].typeReward == product.reward.typeReward)
                    SaveManager.shopDatabase.JSONShop.resources.productSaves[j].isVisible = isVisible;
            }

            SaveManager.UpdateShopDatabase();
        }

        public void Remove(ProductGUI productGUI)
        {
            productsGUIs.Remove(productGUI);
            GameObject.Destroy(productGUI.gameObject);
            Sort();
        }

        public ProductGUI AddProduct(Product product)
        {
            product.isVisible = true;
            ProductData productData = new ProductData();

            productData.typeReward = product.reward.typeReward;
            productData.countReward = -1;
            productData.isVisible = true;

            SaveManager.shopDatabase.JSONShop.resources.productSaves.Add(productData);

            ProductGUI productGUI = GameObject.Instantiate(prefab, content.transform).GetComponent<ProductGUI>();
            productGUI.name = $"Product {productsGUIs.Count}";
            productsGUIs.Add(productGUI);
            productGUI.Init(
                (product) =>
                {
                    if (postBox.ItemInBox() == false && postTube.IsItemFlies() == false)
                    {
                        if (dispensingTasks.Count < MaxObjectFall)
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
                            toastManager.ShowToast("Достигнуто максимальное количество предметов на выдачу");
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
                }, product);

            productGUI.UpdateData(product);
            Sort();
            SaveManager.UpdatePlayerDatabase();
            return productGUI;
        }
    }
}
