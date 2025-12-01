using UnityEngine;

public class AbilityTarget
{
    public HealthComponent Health;
    public Transform Transform;

    public AbilityTarget(HealthComponent health)
    {
        Health = health;
        Transform = health.transform;
    }
}
