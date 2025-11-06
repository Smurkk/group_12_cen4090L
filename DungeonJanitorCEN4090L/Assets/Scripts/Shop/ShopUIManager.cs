using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour
{
    [Header("Data")]
    public List<ShopItemSO> items = new();   // drag your ShopItem assets here

    [Header("Wiring")]
    public Transform gridParent;             // ScrollView/Viewport/Content
    public ShopItemCard cardPrefab;          // Prefabs/UI/ShopItemCard
    public Currency currency;                // GameSystems (Currency)
    public TMP_Text goldText;                // GoldText TMP

    void Start()
    {
        RefreshGold();
        Build();
    }

    void Build()
    {
        foreach (Transform c in gridParent) Destroy(c.gameObject);
        foreach (var item in items)
        {
            var card = Instantiate(cardPrefab, gridParent);
            card.Bind(item, HandleBuy);
        }
    }

    void HandleBuy(ShopItemSO item)
    {
        if (!currency) return;

        if (currency.TrySpend(item.price))
        {
            Debug.Log($"Purchased {item.displayName} for ${item.price}");
            RefreshGold();
        }
        else
        {
            Debug.Log($"Not enough gold for {item.displayName}");
        }
    }

    void RefreshGold()
    {
        if (goldText && currency) goldText.text = $"Gold: {currency.Gold}";
    }
}
