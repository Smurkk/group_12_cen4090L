using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionNodeCard : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Button unlockButton;

    private ProgressionNodeSO data;
    private System.Action<ProgressionNodeSO> onUnlock;

    public void Bind(ProgressionNodeSO node, bool isUnlocked, System.Action<ProgressionNodeSO> onUnlockClicked)
    {
        data = node;
        onUnlock = onUnlockClicked;

        if (icon)
        {
            icon.sprite = node.icon;
        }

        if(nameText) {
            nameText.text = node.displayName;
        }

        if(costText) { 
            costText.text = isUnlocked ? "Unlocked" : $"Cost: {node.cost}"; 
        }

        if(unlockButton)
        {
            unlockButton.interactable = !isUnlocked;
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(() => onUnlock?.Invoke(data));
        }
    }
}
