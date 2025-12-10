using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ProgressionTreeUI : MonoBehaviour
{
    [Header("Data")]
    public List<ProgressionNodeSO> nodes = new();  // all nodes in the tree
    public Experience experience;                  // GameSystems (Experience)
    public TMP_Text xpText;                        // header text "XP: ###"

    [Header("Wiring")]
    public ProgressionNodeCard nodePrefab;         // Prefabs/UI/ProgressionNodeCard

    // Five columns in the progression tree (Content/Column1..Column5)
    public Transform column1;
    public Transform column2;
    public Transform column3;
    public Transform column4;
    public Transform column5;

    // runtime state
    private HashSet<ProgressionNodeSO> unlocked = new();
    private List<ProgressionNodeCard> spawnedCards = new();

    void Start()
    {
        RefreshXP();
        BuildTree();
        RefreshUI();
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
        ClearColumn(column5);
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
            4 => column5,
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
        // Tier 0 is always allowed (xp permitting)
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
            bool hasEnoughXP = experience == null || experience.XP >= node.cost; // optional XP gating for "Unlock" state

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
}
