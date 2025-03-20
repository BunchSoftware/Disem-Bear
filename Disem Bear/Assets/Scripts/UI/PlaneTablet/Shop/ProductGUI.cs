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
        [SerializeField] private Image avatarRewardMask;
        [SerializeField] private Image avatarReward;
        [SerializeField] private Image avatarPriceMask;
        [SerializeField] private Image avatarPrice;
        [SerializeField] private ToolTipTrigger toolTipTrigger;
        private Product product;
        private Action ActionRemove;

        public void Init(Action<Product> ActionBuy, Action ActionRemove, Product product)
        {
            this.product = product;
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
            toolTipTrigger.message = product.description;
            avatarReward.sprite = product.reward.avatarReward;
           
            if(product.price > 0)
            {
                avatarPriceMask.gameObject.SetActive(true);
                avatarPrice.sprite = product.avatarPriceProduct;
            }
            else
            {
                avatarPriceMask.gameObject.SetActive(false);
                avatarPrice.sprite = null;
            }

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

            gameObject.SetActive(product.isVisible);

            countProductText.text = $"{product.reward.countReward}x";
            priceText.gameObject.SetActive(true);

            if(product.price > 0)
                priceText.text = $"{product.price}x";
            else
                priceText.text = $"Бесплатно";
        }

        public Product GetProduct()
        {
            return product;
        }
    }
}
