using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Player playerData;
    [SerializeField] private ResourceManager manaManager;
    [SerializeField] private Transform castPoint;

    private PlayerInput playerInput;
    private InputAction useAbility1Action;
    private InputAction useAbility2Action;
    private InputAction useAbility3Action;

    private float[] abilityCooldowns;

    private void Awake()
    {
        if (playerData == null)
        {
            Debug.LogError("Player SO not assigned to AbilityManager.");
            return;
        }

        if (manaManager == null)
        {
            Debug.LogError("Mana ResourceManager not assigned to AbilityManager.");
            return;
        }

        playerInput = GetComponent<PlayerInput>();

        useAbility1Action = playerInput.actions["UseAbility1"];
        useAbility2Action = playerInput.actions["UseAbility2"];
        useAbility3Action = playerInput.actions["UseAbility3"];

        abilityCooldowns = new float[playerData.EquippedAbilities.Length];

        useAbility1Action.performed += ctx => UseAbility(0);
        useAbility2Action.performed += ctx => UseAbility(1);
        useAbility3Action.performed += ctx => UseAbility(2);
    }
    private void OnEnable()
    {
        useAbility1Action.Enable();
        useAbility2Action.Enable();
        useAbility3Action.Enable();
    }
    private void OnDisable()
    {
        useAbility1Action.Disable();
        useAbility2Action.Disable();
        useAbility3Action.Disable();

    }
    private void Update()
    {
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            if (abilityCooldowns[i] > 0f)
                abilityCooldowns[i] -= Time.deltaTime;
        }
    }
    private void UseAbility(int index)
    {
        try
        {
            AbilityDefinition ability = playerData.GetAbility(index);

            if (ability == null)
            {
                Debug.Log($"No ability equipped in slot {index + 1}");
                return;
            }

            // Check cooldown
            if (abilityCooldowns[index] > 0f)
            {
                Debug.Log($"{ability.name} is on cooldown! {abilityCooldowns[index]:F1}s remaining");
                return;
            }

            // Check mana cost
            if (!manaManager.HasSufficientResources(ability.ResourceCost))
            {
                Debug.Log($"Not enough mana to use {ability.name}! Need: {ability.ResourceCost}, Have: {manaManager.CurrentAmount}");
                return;
            }

            // Execute the ability
            bool success = ability.Use(gameObject, castPoint);

            if (success)
            {
                // Spend mana
                manaManager.SpendResource(ability.ResourceCost);

                // Start cooldown
                abilityCooldowns[index] = ability.Cooldown;

                Debug.Log($"Used {ability.name}! Cooldown: {ability.Cooldown}s, Mana spent: {ability.ResourceCost}");
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.LogError($"UseAbility: Ability index {index} is out of bounds.");
        }
    }
}
