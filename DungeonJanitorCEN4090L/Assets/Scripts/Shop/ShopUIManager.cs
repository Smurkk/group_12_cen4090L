using UnityEngine;
using TMPro;
using System.Collections.Generic;

// Weapon types correspond to the three columns
public enum WeaponType
{
    Sword,
    Bow,
    Staff
}

// Links a ShopItemSO to its weapon type and tier within that type
[System.Serializable]
public class WeaponEntry
{
    public ShopItemSO item;
    public WeaponType type;   // Sword / Bow / Staff
    public int tierIndex;     // 0..4 for the 5 items in each column
}

public class ShopUIManager : MonoBehaviour
{
    [Header("Data")]
    // Old generic items list (armor, potions, etc.) – currently unused, but kept in case needed later.
    public List<ShopItemSO> items = new();

    [Header("Wiring")]
    public Transform gridParent;             // ScrollView/Viewport/Content
    public ShopItemCard cardPrefab;          // Prefabs/UI/ShopItemCard
    public Currency currency;                // GameSystems (Currency)
    public TMP_Text goldText;                // GoldText TMP

    [Header("Weapon Shop")]
    public Transform swordColumn;            // parent for swords
    public Transform bowColumn;              // parent for bows
    public Transform staffColumn;            // parent for staffs

    public List<WeaponEntry> weapons = new();    // 15 entries: 5 swords, 5 bows, 5 staffs

    // Runtime state
    private HashSet<ShopItemSO> purchasedWeapons = new();
    private Dictionary<WeaponType, ShopItemSO> equippedByType = new();
    private List<ShopItemCard> weaponCards = new();

    void Awake()
    {
        // If columns weren't assigned in the Inspector, try to auto-find them as children of gridParent
        if (gridParent != null)
        {
            if (swordColumn == null)
            {
                var t = gridParent.Find("SwordColumn");
                if (t != null) swordColumn = t;
            }

            if (bowColumn == null)
            {
                var t = gridParent.Find("BowColumn");
                if (t != null) bowColumn = t;
            }

            if (staffColumn == null)
            {
                var t = gridParent.Find("StaffColumn");
                if (t != null) staffColumn = t;
            }
        }
    }

    void Start()
    {
        RefreshGold();
        // Build();                // old generic shop – not needed for weapon grid right now
        BuildWeaponShop();          // build the 3-column weapon grid
    }

    // =====================
    //  OLD GENERIC SHOP (unused for now)
    // =====================
    void Build()
    {
        if (gridParent == null) return;

        foreach (Transform c in gridParent)
            Destroy(c.gameObject);

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
        if (goldText && currency)
            goldText.text = $"Gold: {currency.Gold}";
    }

    // ==========================
    //  WEAPON SHOP (3 COLUMNS)
    // ==========================

    void BuildWeaponShop()
    {
        // Clear existing children in each column
        ClearColumn(swordColumn);
        ClearColumn(bowColumn);
        ClearColumn(staffColumn);

        weaponCards.Clear();

        // Instantiate a card for each weapon entry
        foreach (var weapon in weapons)
        {
            if (weapon == null || weapon.item == null)
                continue;

            Transform parent = GetColumnForType(weapon.type);
            if (!parent) continue;

            var card = Instantiate(cardPrefab, parent);
            card.Bind(weapon.item, HandleWeaponBuy);

            weaponCards.Add(card);
        }

        // Set initial visual states
        RefreshAllWeaponCards();
    }

    void ClearColumn(Transform column)
    {
        if (!column) return;
        foreach (Transform c in column)
            Destroy(c.gameObject);
    }

    Transform GetColumnForType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return swordColumn;
            case WeaponType.Bow: return bowColumn;
            case WeaponType.Staff: return staffColumn;
            default: return null;
        }
    }

    // Called by ShopItemCard when a weapon card's Buy button is clicked
    void HandleWeaponBuy(ShopItemSO item)
    {
        if (item == null) return;

        // Look up its WeaponEntry to know type/tier
        WeaponEntry entry = GetWeaponEntryForItem(item);
        if (entry == null)
        {
            Debug.LogWarning($"No WeaponEntry found for item {item.name}");
            return;
        }

        // Enforce progression: must buy earlier tiers of this type first
        if (!CanBuy(entry))
        {
            Debug.Log($"Cannot buy {item.displayName} yet – purchase earlier {entry.type} tiers first.");
            return;
        }

        if (!currency) return;

        if (currency.TrySpend(item.price))
        {
            Debug.Log($"Purchased {item.displayName} ({entry.type} tier {entry.tierIndex}) for ${item.price}");
            RefreshGold();

            // Mark as purchased and equipped for this type
            purchasedWeapons.Add(item);
            equippedByType[entry.type] = item;

            // Update card visuals
            RefreshAllWeaponCards();
        }
        else
        {
            Debug.Log($"Not enough gold for {item.displayName}");
        }
    }

    WeaponEntry GetWeaponEntryForItem(ShopItemSO item)
    {
        foreach (var w in weapons)
        {
            if (w != null && w.item == item)
                return w;
        }
        return null;
    }

    // Can this weapon be bought, based on tier progression?
    bool CanBuy(WeaponEntry entry)
    {
        // First in the column can always be bought
        if (entry.tierIndex == 0)
            return true;

        // For tier n, require tiers 0..(n-1) of the same type to be purchased
        for (int i = 0; i < entry.tierIndex; i++)
        {
            var prev = GetWeaponByTypeAndTier(entry.type, i);
            if (prev == null || !purchasedWeapons.Contains(prev.item))
                return false;
        }

        return true;
    }

    WeaponEntry GetWeaponByTypeAndTier(WeaponType type, int tierIndex)
    {
        foreach (var w in weapons)
        {
            if (w != null && w.type == type && w.tierIndex == tierIndex)
                return w;
        }
        return null;
    }

    void RefreshAllWeaponCards()
    {
        foreach (var card in weaponCards)
        {
            if (card == null) continue;

            var item = card.Item;
            if (item == null)
            {
                card.SetAsLockedNotAvailable();
                continue;
            }

            var entry = GetWeaponEntryForItem(item);
            if (entry == null)
            {
                card.SetAsLockedNotAvailable();
                continue;
            }

            bool purchased = purchasedWeapons.Contains(item);
            bool canBuy = CanBuy(entry);

            equippedByType.TryGetValue(entry.type, out ShopItemSO equipped);

            if (purchased)
            {
                if (equipped == item)
                {
                    // Latest purchased of this type = equipped
                    card.SetAsEquipped();
                }
                else
                {
                    // Previously purchased in this type = locked with icon
                    card.SetAsLockedPurchased();
                }
            }
            else
            {
                if (canBuy)
                {
                    // Next available in the chain
                    card.SetAsBuyable();
                }
                else
                {
                    // Can't buy yet – locked behind earlier tiers
                    card.SetAsLockedNotAvailable();
                }
            }
        }
    }
}

