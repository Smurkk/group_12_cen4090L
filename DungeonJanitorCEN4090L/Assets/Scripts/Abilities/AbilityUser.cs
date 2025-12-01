// AbilityUser.cs
using UnityEngine;

[RequireComponent(typeof(ResourceManager))]
public class AbilityUser : MonoBehaviour
{
    [SerializeField] private AbilityDefinition equippedAbility;
    [SerializeField] private Transform castPoint;
    [SerializeField] private float projectileLifetime;

    private ResourceManager resourceManager;
    private float cooldownTimer;

    public float SpellPower; 

    private void Awake()
    {
        resourceManager = GetComponent<ResourceManager>();
        if (castPoint == null) castPoint = this.transform;
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        // temp input for testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryUseEquippedAbility();
        }
    }

    public void SetEquippedAbility(AbilityDefinition ability)
    {
        equippedAbility = ability;
    }

    public bool TryUseEquippedAbility()
    {
        if (equippedAbility == null)
        {
            Debug.LogWarning("No equipped ability");
            return false;
        }

        if (cooldownTimer > 0f)
        {
            Debug.Log($"{equippedAbility.Name} is on cooldown");
            return false;
        }

        if (!resourceManager.HasSufficientResources(equippedAbility.ResourceCost))
        {
            Debug.Log("Not enough resource");
            return false;
        }

        // pay resource and start cooldown
        resourceManager.SpendResource(equippedAbility.ResourceCost);
        cooldownTimer = equippedAbility.Cooldown;

        switch (equippedAbility.Targeting)
        {
            case TargetingType.Projectile:
                LaunchProjectileAbility(equippedAbility);
                break;
            case TargetingType.RaycastSingle:
                // you could implement raycast-target instant hit abilities
                break;
            case TargetingType.Self:
                // apply effects to self
                ApplyEffectsToTarget(this.gameObject, equippedAbility.Effects);
                break;
                // area, point, etc.
        }

        return true;
    }

    private void LaunchProjectileAbility(AbilityDefinition ability)
    {
        if (ability.ProjectilePrefab == null)
        {
            Debug.LogError($"Ability {ability.Name} has no projectile prefab assigned.");
            return;
        }

        // instantiate projectile
        GameObject projGO = Instantiate(ability.ProjectilePrefab, castPoint.position, castPoint.rotation);
        var proj = projGO.GetComponent<Projectile>();
        if (proj == null)
        {
            Debug.LogError("Projectile prefab requires a Projectile component.");
            Destroy(projGO);
            return;
        }

        // configure projectile runtime params (speed comes from ability so it's modifiable per-ability)
        proj.Caster = this.gameObject;
        proj.Speed = ability.ProjectileSpeed;
        proj.Lifetime = projectileLifetime;
        proj.Effects = ability.Effects;
        proj.HitLayers = ability.HitLayers;
    }

    private void ApplyEffectsToTarget(GameObject target, EffectDefinition[] effects)
    {
        if (effects == null || target == null) return;

        foreach (var ed in effects)
        {
            if (ed == null || ed.effect == null) continue;
            var runtimeEffect = ed.effect.CreateEffectInstance();
            runtimeEffect.Apply(this, target, ed, target.transform.position);
        }
    }
}
