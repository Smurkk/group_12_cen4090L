using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionNodeCard : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Button unlockButton;

    // label text on the button ("Unlock", "Unlocked", "Locked")
    public TMP_Text buttonLabel;

    // lock icon that we can toggle on/off
    public GameObject lockIcon;

    private ProgressionNodeSO data;
    private System.Action<ProgressionNodeSO> onUnlock;

    // Expose which node this card represents
    public ProgressionNodeSO Node => data;

    // NEW main Bind: no isUnlocked flag; manager controls state via helper methods.
    public void Bind(ProgressionNodeSO node, System.Action<ProgressionNodeSO> onUnlockClicked)
    {
        data = node;
        onUnlock = onUnlockClicked;

        if (icon && node.icon != null)
        {
            icon.sprite = node.icon;
        }

        if (nameText)
        {
            nameText.text = node.displayName;
        }

        if (costText)
        {
            costText.text = $"Cost: {node.cost}";
        }

        if (unlockButton)
        {
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(() => onUnlock?.Invoke(data));
        }

        // Default; manager should immediately override with the correct state.
        SetAsLockedNotAvailable();
    }

    // 🔁 OVERLOAD for old code: supports Bind(node, isUnlocked, callback)
    public void Bind(ProgressionNodeSO node, bool isUnlocked, System.Action<ProgressionNodeSO> onUnlockClicked)
    {
        // Use the main Bind for shared setup
        Bind(node, onUnlockClicked);

        // Then set initial state based on the bool flag
        if (isUnlocked)
        {
            SetAsUnlocked();
        }
        else
        {
            SetAsUnlockable(); // or LockedNotAvailable, depending on how your old logic worked
        }
    }

    // =======================
    //  STATE VISUAL HELPERS
    // =======================

    // Node cannot be unlocked yet (previous tier not unlocked)
    public void SetAsLockedNotAvailable()
    {
        if (unlockButton) unlockButton.interactable = false;
        if (buttonLabel) buttonLabel.text = "Locked";
        if (lockIcon) lockIcon.SetActive(true);
        if (costText) costText.text = $"Cost: {data?.cost ?? 0}";
    }

    // Node is available to unlock (previous tiers satisfied, you can afford it etc.)
    public void SetAsUnlockable()
    {
        if (unlockButton) unlockButton.interactable = true;
        if (buttonLabel) buttonLabel.text = "Unlock";
        if (lockIcon) lockIcon.SetActive(false);
        if (costText) costText.text = $"Cost: {data?.cost ?? 0}";
    }

    // Node has been unlocked
    public void SetAsUnlocked()
    {
        if (unlockButton) unlockButton.interactable = false;
        if (buttonLabel) buttonLabel.text = "Unlocked";
        if (lockIcon) lockIcon.SetActive(false);
        if (costText) costText.text = "Unlocked";
    }
}


