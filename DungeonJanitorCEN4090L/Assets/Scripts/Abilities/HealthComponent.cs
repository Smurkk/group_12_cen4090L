using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        Debug.Log($"{name} took {amount} dmg. HP: {CurrentHealth}/{MaxHealth}");
        if (CurrentHealth <= 0f) Die();
    }

    private void Die()
    {
        Debug.Log($"{name} died.");

        // TODO: Add death logic (play animation, drop loot, etc.)
    }
}
