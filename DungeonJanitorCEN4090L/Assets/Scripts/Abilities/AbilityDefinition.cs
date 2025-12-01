using UnityEngine;

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

    // [SerializeField] private TargetingType targetingType; // Temporary comment so I  can compile in the meantime.
    [SerializeField, 
        Tooltip("Max distance for targeting. Only used for RaycastSingle or Projectile abilities.")] 
    private float? range;

    public string Name => abilityName;
    public string Description => description;
    public Sprite Icon => icon;
    public float ResourceCost => resourceCost;
    public float Cooldown => cooldown;
}
