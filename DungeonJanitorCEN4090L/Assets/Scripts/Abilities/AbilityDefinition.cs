using UnityEngine;

// TODO: Move to location that most makes sense
[CreateAssetMenu(menuName = "Scriptable Objects/AbilityDefinition")]
public class AbilityDefinition : ScriptableObject
{
    [Header("Basic info")]
    [SerializeField] private string abilityName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Cooldown & Resources")]
    [SerializeField] private float cooldown;
    [SerializeField] private float resourceCost;


    [Header("Targeting")]
    [SerializeField] private TargetingType targetingType;
    [SerializeField, Tooltip("Max distance for targeting. Set to 0 if not applicable.")] private float range; // Cannot be nullable, so set to 0 if not applicable
    [SerializeField] private LayerMask hitLayers;

    [Header("Effects")]
    [SerializeField] private EffectDefinition[] effects;

    [Header("Projectile (if applicable)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    // Basic Info
    public string Name => abilityName;
    public string Description => description;
    public Sprite Icon => icon;

    // Cooldown & Resources
    public float Cooldown => cooldown;
    public float ResourceCost => resourceCost;

    // Targeting
    public TargetingType Targeting => targetingType;
    public float? Range => range;
    public LayerMask HitLayers => hitLayers;

    // Effects
    public EffectDefinition[] Effects => effects;

    // Projectile (if applicable)
    public GameObject ProjectilePrefab => projectilePrefab;
    public float ProjectileSpeed => projectileSpeed;
}

public enum TargetingType
{
    Self,
    RaycastSingle,
    AreaOfEffect,
    Projectile
}