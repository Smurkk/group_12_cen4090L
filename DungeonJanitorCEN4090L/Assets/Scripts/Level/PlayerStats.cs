using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Dummy XP value
    public int experience = 0;

    // Dummy XP function so CleanableObject can call it
    public void AddExperience(int amount)
    {
        Debug.Log($"[Dummy PlayerStats] AddExperience({amount}) called.");
        experience += amount;
    }
}
