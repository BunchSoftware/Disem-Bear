using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProductGUI : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Text headerText;
    [SerializeField] private Text rewardText;
    [SerializeField] private Text countProductText;
    [SerializeField] private Image avatar;
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
        rewardText.text = product.rewardText;
        avatar.sprite = product.avatar;
        if (product.countProduct == -1)
            countProductText.gameObject.SetActive(false);
        else
        {
            if (product.countProduct == 0)
            {
                ActionRemove?.Invoke();
                return;
            }
            countProductText.gameObject.SetActive(true);
            countProductText.text = $"{product.countProduct}x";
        }
    }

    public Product GetProduct()
    {
        return product;
    }
}
