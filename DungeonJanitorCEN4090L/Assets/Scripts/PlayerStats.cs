using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Player playerData;   // ScriptableObject
    [SerializeField] private healthBar healthBar; // Scene reference

    private void Start()
    {
        playerData.CurrentHealth = playerData.maxHealth;
        healthBar.SetMaxHealth(playerData.maxHealth);
        healthBar.SetHealth(playerData.CurrentHealth);
    }


    /*
    public void TakeDamage(int amount)
    {
        playerData.CurrentHealth -= amount;
        healthBar.SetHealth(playerData.CurrentHealth);
    }
    */
}
