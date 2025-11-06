using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemCard : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    private ShopItemSO data;
    private System.Action<ShopItemSO> onBuy;

    public void Bind(ShopItemSO item, System.Action<ShopItemSO> onBuyClicked)
    {
        data = item;
        onBuy = onBuyClicked;

        if(icon)
        {
            icon.sprite = item.icon;
        }

        if(nameText)
        {
            nameText.text = item.displayName;
        }

        if(priceText)
        {
            priceText.text = $"${item.price}";
        }

        if(buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuy?.Invoke(data));
        }
    }
}
