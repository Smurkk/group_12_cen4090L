using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{

    [Header("Equipment")]
    [SerializeField, Tooltip("Should always be equipped")] private Weapon broomWeapon;

    [Header("Combat Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxMana = 100;

    public int CurrentHealth;
    public int CurrentMana;

    [Header("Character Stats")]
    public int Level = 1;
    public int Experience;

    public double Damage;
    public double BaseStrength;
    public float Speed;

    [Header("Equipment Slots")]
    public Weapon Weapon;
    public Armor Armor;

    [Header("Abilities")]
    [SerializeField] private int maxAbilitySlots = 3;
    [SerializeField] private AbilityDefinition[] equippedAbilities;
    public AbilityDefinition[] EquippedAbilities => equippedAbilities;

    // Properties
    public int MaxHealth => maxHealth;
    public int MaxMana => maxMana;
    public Weapon BroomWeapon => broomWeapon = broomWeapon != null ? broomWeapon : Resources.Load<Weapon>("Weapons/Melee/Broom");

    public float HealthPercentage => maxHealth > 0 ? (CurrentHealth / MaxHealth) : 0f;
    public bool IsDead => CurrentHealth <= 0;

    public void Initialize()
    {
        CurrentHealth = maxHealth;
        CurrentMana = maxMana;
        Level = 1;
        Experience = 0;

        if (equippedAbilities == null || equippedAbilities.Length != maxAbilitySlots)
        {
            equippedAbilities = new AbilityDefinition[maxAbilitySlots];
        }
    }

    public AbilityDefinition GetAbility(int index)
    {
        if (index < 0 || index >= equippedAbilities.Length)
        {
            Debug.LogError($"GetAbility: index out of range. Index Value: {index} | Max Array Index: {equippedAbilities.Length-1}");
            return null;
        }
        return equippedAbilities[index];
    }

    public void EquipAbility(AbilityDefinition ability, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedAbilities.Length)
        {
            Debug.LogError($"EquipAbility: Invalid ability slot index: {slotIndex}");
            return;
        }
        equippedAbilities[slotIndex] = ability;
    }
}
