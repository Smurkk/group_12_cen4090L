using UnityEngine;

[CreateAssetMenu(menuName = "Progression/Node", fileName = "ProgressionNode")]
public class ProgressionNodeSO : ScriptableObject
{
    [Header("Display")]
    public string displayName;
    public Sprite icon;
    public int cost;
    [TextArea] public string placeholderEffect;

    [Header("Tree Position")]
    // Column this node belongs to (0–4 for your 5 columns)
    public int columnIndex;
    // Tier within that column (0..4)
    public int tierIndex;
}