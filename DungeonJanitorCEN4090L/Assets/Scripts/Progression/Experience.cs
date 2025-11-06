using UnityEngine;

public class Experience : MonoBehaviour
{
    public int startingXP = 500;
    public int XP { get; private set; }

    void Awake() => XP = startingXP;

    public bool TrySpend(int amount)
    {
        if (XP < amount) return false;
        XP -= amount;
        return true;
    }

    public void Add(int amount) => XP += amount;
}
