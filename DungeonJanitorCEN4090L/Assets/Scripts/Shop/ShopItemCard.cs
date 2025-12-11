using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemCard : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    // label on the buy button
    public TMP_Text buyButtonLabel;

    // lock icon we can toggle on/off
    public GameObject lockIcon;

    private ShopItemSO data;
    private System.Action<ShopItemSO> onBuy;

    // 🔹 NEW: expose which item this card is bound to
    public ShopItemSO Item => data;

    public void Bind(ShopItemSO item, System.Action<ShopItemSO> onBuyClicked)
    {
        data = item;
        onBuy = onBuyClicked;

        if (icon)
        {
            icon.sprite = item.icon;
        }

        if (nameText)
        {
            nameText.text = item.displayName;
        }

        if (priceText)
        {
            // keep your existing formatting
            priceText.text = $"${item.price}";
        }

        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuy?.Invoke(data));
        }

        // Default visual state when first bound
        SetAsBuyable();
    }

    private void OnEnable()
    {
        // Safety: ensure UI is consistent if object gets reused
        if (data != null)
        {
            SetAsBuyable();
        }
    }

    // =======================
    //  STATE VISUAL HELPERS
    // =======================

    /// <summary>
    /// Item is available to purchase (normal "Buy" state).
    /// </summary>
    public void SetAsBuyable()
    {
        if (buyButton) buyButton.interactable = true;
        if (buyButtonLabel) buyButtonLabel.text = "Buy";
        if (lockIcon) lockIcon.SetActive(false);
    }

    /// <summary>
    /// This item is currently equipped (for this weapon type).
    /// </summary>
    public void SetAsEquipped()
    {
        if (buyButton) buyButton.interactable = false;
        if (buyButtonLabel) buyButtonLabel.text = "Equipped";
        if (lockIcon) lockIcon.SetActive(false);
    }

    /// <summary>
    /// This item was purchased before, but is no longer the equipped one
    /// for this type. Show it as locked with no text.
    /// </summary>
    public void SetAsLockedPurchased()
    {
        if (buyButton) buyButton.interactable = false;
        if (buyButtonLabel) buyButtonLabel.text = "";
        if (lockIcon) lockIcon.SetActive(true);
    }

    /// <summary>
    /// This item is not yet available (must buy previous tiers first).
    /// Show "Locked" + lock icon.
    /// </summary>
    public void SetAsLockedNotAvailable()
    {
        if (buyButton) buyButton.interactable = false;
        if (buyButtonLabel) buyButtonLabel.text = "Locked";
        if (lockIcon) lockIcon.SetActive(true);
    }
}

