using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AbilityUser abilityUser;
    [SerializeField] private Transform  castPoint; // optional, can assign via AbilityUser

    private PlayerInput playerInput;
    private InputAction fireballAction;
    private InputAction healAction;

    private void Awake()
    {
        if (abilityUser == null)
            abilityUser = GetComponent<AbilityUser>();
        playerInput = GetComponent<PlayerInput>();

        if(playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on PlayerController GameObject.");
            return;
        }

        fireballAction = playerInput.actions["UseAbility"]; 
        fireballAction.performed += OnFireballPerformed;

        healAction = playerInput.actions["Heal"];
        healAction.performed += OnHealPerformed;
    }

    private void OnEnable()
    {
        fireballAction?.Enable();
        healAction?.Enable();
    }

    private void OnDisable()
    {
        fireballAction?.Disable();
        healAction?.Disable();
    }

    private void OnFireballPerformed(InputAction.CallbackContext context)
    {
        // Cast the ability in the player's looking direction
        abilityUser.TryUseEquippedAbility();
    }
    private void OnHealPerformed(InputAction.CallbackContext context)
    {
        abilityUser.TryUseEquippedAbility();
    }
}
