using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ProgressionTreeUI : MonoBehaviour
{
    [Header("Data")]
    public List<ProgressionNodeSO> nodes = new();

    [Header("Wiring")]
    public Transform gridParent;              // ProgressionScroll/Viewport/Content
    public ProgressionNodeCard nodePrefab;    // Prefabs/UI/ProgressionNodeCard
    public Experience experience;             // GameSystems (Experience)
    public TMP_Text xpText;                   // header text "XP: ###"

    private HashSet<ProgressionNodeSO> unlocked = new();

    void Start()
    {
        RefreshXP();
        Build();
    }

    void Build()
    {
        foreach (Transform c in gridParent) Destroy(c.gameObject);
        foreach (var n in nodes)
        {
            var card = Instantiate(nodePrefab, gridParent);
            bool isUnlocked = unlocked.Contains(n);
            card.Bind(n, isUnlocked, HandleUnlock);
        }
    }

    void HandleUnlock(ProgressionNodeSO node)
    {
        if (unlocked.Contains(node)) return;
        if (!experience) return;

        if (experience.TrySpend(node.cost))
        {
            unlocked.Add(node);
            RefreshXP();
            Build(); // update "Unlocked" labels / disabled buttons
        }
        else
        {
            Debug.Log($"Not enough XP to unlock {node.displayName}");
        }
    }

    void RefreshXP()
    {
        if (xpText && experience) xpText.text = $"XP: {experience.XP}";
    }
}
