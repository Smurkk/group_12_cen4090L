public interface IEffect
{
    /// <summary>
    /// Apply the effect at runtime.
    /// caster - the AbilityUser who cast this ability.
    /// target  - GameObject hit (may be null for area/self effects).
    /// def     - the EffectDefinition data from the AbilityDefinition.
    /// hitPoint- the world point where effect is applied (optional).
    /// </summary>
    void Apply(AbilityUser caster, UnityEngine.GameObject target, EffectDefinition def, UnityEngine.Vector3 hitPoint);
}
