using UnityEngine;

[CreateAssetMenu(menuName = "Progression/Node", fileName = "ProgressionNode")]
public class ProgressionNodeSO : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public int cost;
    [TextArea] public string placeholderEffect;
}
