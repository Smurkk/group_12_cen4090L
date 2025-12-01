using Unity.Hierarchy;
using UnityEngine;

[System.Serializable]
public class EffectDefinition
{
    public EffectSO effect;

    [Tooltip("How powerful the effect is (how much damage dealt/healing recieved.")]
    public float magnitude;

    [Tooltip("How long the effect lasts (in seconds). 0 for instant, >0 for over-time effect.")]
    public float duration;

    [Tooltip("Scaling factor to modify the effect. Set to 1 for no scaling.")]
    public float scalingFactor;

}
