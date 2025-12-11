using Unity.Hierarchy;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;

public interface IHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsDead { get; }

    void Heal(float amount);
    void TakeDamage(float amount);
}

public class HealthComponent : MonoBehaviour, IHealth
{
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float currentHealth;


    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealed;
    public UnityEvent<float> OnDamaged;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f; // Returns the truth value of the statement (currentHealth <= 0f)
    
    public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f; // TODO: I don't think this is necessary? Unless we want to have a health bar

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnDamaged?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log($"{name} took {amount} damage. Current health: {currentHealth}/{maxHealth}");
        if (IsDead) Die();
    }

    public void Heal(float amount)
    {
        if(IsDead || amount <= 0f) return;
        
        float healthBefore = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);

        //Added this var in case we ever want to add a stat tracker for total healing lol
        float actualHealing = currentHealth - healthBefore;

        if(actualHealing > 0f)
        {
            OnHealed?.Invoke(actualHealing);
            OnHealthChanged?.Invoke(currentHealth);
            Debug.Log($"{name} healed for {actualHealing} health. Current health: {currentHealth}/{maxHealth}");
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{name} died.");
        // TODO: Add death logic (play animation, drop loot, etc.)

    }

    // Added this in case we want to fully heal the player at some point.
    public void FullHeal()
    {
        Heal(maxHealth);
    }




    // For testing purposes only
    [ContextMenu("Test Take 5 Damage")]
    private void TestTakeDamage()
    {
        TakeDamage(5f);
    }
    [ContextMenu("Test Heal 5 Health")]
    private void TestHeal()
    {
        Heal(5f);
    }
}
