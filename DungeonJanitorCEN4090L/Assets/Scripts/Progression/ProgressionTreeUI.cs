using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ProgressionTreeUI : MonoBehaviour
{
    private const string SaveKey_Progression = "ProgressionTree_Unlocked";

    [Header("Data")]
    public List<ProgressionNodeSO> nodes = new();  // all nodes in the tree
    public Experience experience;                  // GameSystems (Experience)
    public TMP_Text xpText;                        // header text "XP: ###"

    [Header("Wiring")]
    public ProgressionNodeCard nodePrefab;         // Prefabs/UI/ProgressionNodeCard

    // Four columns in the progression tree (Content/Column1..Column4)
    public Transform column1;
    public Transform column2;
    public Transform column3;
    public Transform column4;

    [Header("Passive Bonuses")]
    [Tooltip("Optional: if assigned, will recalc passive multipliers whenever progression is loaded/unlocked.")]
    public PassiveBonusManager passiveBonusManager;

    // runtime state
    private HashSet<ProgressionNodeSO> unlocked = new();
    private List<ProgressionNodeCard> spawnedCards = new();

    void Start()
    {
        // If you later add XP save/load, you could call experience.LoadXP() here.
        LoadProgress();

        // NEW: Recalculate passive bonuses from saved unlocks
        RecalculatePassives();

        RefreshXP();
        BuildTree();
        RefreshUI();
    }

    // Optional helper: lets other scripts ask what’s unlocked without exposing the HashSet
    public List<ProgressionNodeSO> GetUnlockedNodes()
    {
        return new List<ProgressionNodeSO>(unlocked);
    }

    // =============================
    // BUILD THE TREE
    // =============================
    void BuildTree()
    {
        ClearColumns();
        spawnedCards.Clear();

        foreach (var node in nodes)
        {
            if (node == null) continue;

            Transform parent = GetColumn(node.columnIndex);
            if (parent == null) continue;

            var card = Instantiate(nodePrefab, parent);
            // Bind with callback; visual state will be set in RefreshUI()
            card.Bind(node, HandleUnlock);

            spawnedCards.Add(card);
        }
    }

    void ClearColumns()
    {
        ClearColumn(column1);
        ClearColumn(column2);
        ClearColumn(column3);
        ClearColumn(column4);
    }

    void ClearColumn(Transform t)
    {
        if (!t) return;

        foreach (Transform child in t)
        {
            Destroy(child.gameObject);
        }
    }

    Transform GetColumn(int index)
    {
        return index switch
        {
            0 => column1,
            1 => column2,
            2 => column3,
            3 => column4,
            _ => null
        };
    }

    // =============================
    // UNLOCK LOGIC
    // =============================
    void HandleUnlock(ProgressionNodeSO node)
    {
        if (node == null) return;
        if (unlocked.Contains(node)) return;
        if (!experience) return;

        // Check tier progression first
        if (!CanUnlock(node))
        {
            Debug.Log($"Cannot unlock {node.displayName} yet – unlock earlier tiers in this column first.");
            return;
        }

        // Check XP cost
        if (experience.TrySpend(node.cost))
        {
            unlocked.Add(node);
            SaveProgress();

            // NEW: Apply passive changes immediately after purchase
            RecalculatePassives();

            RefreshXP();
            RefreshUI();
        }
        else
        {
            Debug.Log($"Not enough XP to unlock {node.displayName}");
        }
    }

    // Tier logic: must unlock all earlier tiers in the same column
    bool CanUnlock(ProgressionNodeSO node)
    {
        // Tier 0 is always allowed (XP permitting)
        if (node.tierIndex == 0)
            return true;

        // For tier n, require tiers 0..(n-1) in same column to be unlocked
        for (int i = 0; i < node.tierIndex; i++)
        {
            var prev = GetNode(node.columnIndex, i);
            if (prev == null || !unlocked.Contains(prev))
                return false;
        }

        return true;
    }

    ProgressionNodeSO GetNode(int columnIndex, int tierIndex)
    {
        foreach (var n in nodes)
        {
            if (n == null) continue;
            if (n.columnIndex == columnIndex && n.tierIndex == tierIndex)
                return n;
        }
        return null;
    }

    // =============================
    // SAVE / LOAD
    // =============================
    void SaveProgress()
    {
        if (nodes == null || nodes.Count == 0)
        {
            PlayerPrefs.DeleteKey(SaveKey_Progression);
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder(nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            bool isUnlocked = node != null && unlocked.Contains(node);
            sb.Append(isUnlocked ? '1' : '0');
        }

        PlayerPrefs.SetString(SaveKey_Progression, sb.ToString());
        PlayerPrefs.Save();
        // Debug.Log($"Saved progression: {sb}");
    }

    void LoadProgress()
    {
        unlocked.Clear();

        string data = PlayerPrefs.GetString(SaveKey_Progression, string.Empty);
        if (string.IsNullOrEmpty(data))
            return;

        int len = Mathf.Min(data.Length, nodes.Count);
        for (int i = 0; i < len; i++)
        {
            if (data[i] == '1' && nodes[i] != null)
            {
                unlocked.Add(nodes[i]);
            }
        }

        // Debug.Log($"Loaded progression: {data}");
    }

    // =============================
    // PASSIVES
    // =============================
    void RecalculatePassives()
    {
        if (passiveBonusManager == null) return;

        // Convert HashSet -> List for the manager
        var unlockedList = new List<ProgressionNodeSO>(unlocked);
        passiveBonusManager.Recalculate(unlockedList);
    }

    // =============================
    // UI REFRESH
    // =============================
    void RefreshUI()
    {
        foreach (var card in spawnedCards)
        {
            if (card == null) continue;

            var node = card.Node;
            if (node == null)
            {
                card.SetAsLockedNotAvailable();
                continue;
            }

            bool isUnlocked = unlocked.Contains(node);
            bool canUnlockByTier = CanUnlock(node);
            bool hasEnoughXP = experience == null || experience.XP >= node.cost;

            if (isUnlocked)
            {
                card.SetAsUnlocked();
            }
            else if (canUnlockByTier && hasEnoughXP)
            {
                card.SetAsUnlockable();
            }
            else
            {
                card.SetAsLockedNotAvailable();
            }
        }
    }

    void RefreshXP()
    {
        if (xpText && experience)
            xpText.text = $"XP: {experience.XP}";
    }

    public void ResetProgression()
    {
        // Clear runtime state
        unlocked.Clear();

        // Remove saved data
        PlayerPrefs.DeleteKey(SaveKey_Progression);
        PlayerPrefs.Save();

        // NEW: reset passives back to baseline (no unlocks)
        RecalculatePassives();

        // Rebuild visuals to reflect locked state
        RefreshUI();

        Debug.Log("Progression tree reset.");
    }
}



