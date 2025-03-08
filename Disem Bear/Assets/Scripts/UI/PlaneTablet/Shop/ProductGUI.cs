using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PlaneTablet.Shop
{
    public class ProductGUI : MonoBehaviour
    {
        [SerializeField] private Button buyButton;
        [SerializeField] private Text headerText;
        [SerializeField] private Text priceText;
        [SerializeField] private Text countProductText;
        [SerializeField] private Image avatarReward;
        [SerializeField] private Image avatarPrice;
        [SerializeField] private Image background;
        private Product product;
        private Action ActionRemove;

        public void Init(Action<Product> ActionBuy, Action ActionRemove, Product product)
        {
            UpdateData(product);

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                ActionBuy?.Invoke(product);
            });
            this.ActionRemove = ActionRemove;
        }

        public void UpdateData(Product product)
        {
            this.product = product;
            headerText.text = product.header;
            avatarReward.sprite = product.reward.avatarReward;
            avatarPrice.sprite = product.avatarPriceProduct;

            if (product.reward.countReward == -1)
                countProductText.gameObject.SetActive(false);
            else
            {
                if (product.reward.countReward == 0)
                {
                    ActionRemove?.Invoke();
                    return;
                }
            }

            countProductText.text = $"{product.reward.countReward}x";
            priceText.gameObject.SetActive(true);
            priceText.text = $"{product.price}x";
        }

        public Product GetProduct()
        {
            return product;
        }
    }
}
