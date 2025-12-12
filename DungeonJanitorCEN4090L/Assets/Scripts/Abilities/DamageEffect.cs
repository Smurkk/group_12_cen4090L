using UnityEngine;

/// <summary>
/// Runtime logic that handles dealing damage by reading EffectDefinition and caster stats.
/// </summary>
public class DamageEffect : IEffect
{
    public void Apply(AbilityUser caster, GameObject target, EffectDefinition def, Vector3 hitPoint)
    {
        if (target == null) return;

        var health = target.GetComponent<HealthComponent>();
        if (health == null) return;

        int finalDamage = (int)def.GetScaledMagnitude();

        health.TakeDamage(finalDamage);

        Debug.Log($"{caster?.name ?? "Unknown"} dealt {finalDamage} damage to {target.name}");
    }
}
