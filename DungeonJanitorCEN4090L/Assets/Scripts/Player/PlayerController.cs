using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AbilityUser abilityUser;
    [SerializeField] private Transform  castPoint; // optional, can assign via AbilityUser

    private PlayerInput playerInput;


    private InputAction ability1Action;
    private InputAction ability2Action;
    private InputAction ability3Action;

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

        
        ability1Action = playerInput.actions["UseAbility1"]; 
        ability1Action.performed += OnFireballPerformed;

        ability2Action = playerInput.actions["UseAbility2"];
        ability2Action.performed += OnHealPerformed;

        ability3Action = playerInput.actions["UseAbility3"];
        ability3Action.performed += OnHealPerformed;

    }

    private void OnEnable()
    {
        ability1Action?.Enable();
        ability2Action?.Enable();
        ability3Action?.Enable();
    }

    private void OnDisable()
    {
        ability1Action?.Disable();
        ability2Action?.Disable();
        ability3Action?.Disable();
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
