using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{

    [SerializeField, Tooltip("Should always be equipped")] private Weapon broomWeapon; // Always equipped
    
    public int Health;
    public int Mana;
    public int Level;
    public int Experience;

    public double Damage;
    public double BaseStrength;

    public float Speed;

    public Weapon Weapon;
    public Weapon BroomWeapon => broomWeapon ??= Resources.Load<Weapon>("Weapons/Melee/Broom");

    public Armor Armor;

    public AbilityDefinition[] Abilities;

    public Player()
    {

    }
    ~Player()
    {
        // Might not be necessary?
    }


}
