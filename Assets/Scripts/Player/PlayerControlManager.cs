using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Singleton for player input so can use in any script
public class PlayerControlManager : MonoBehaviour
{
    public PlayerControls playerControls;

    private InputAction aim;
    private InputAction move;
    private InputAction jump;
    private InputAction dash;
    private InputAction interact;
    private InputAction inventory;
    private InputAction primaryAttack;
    private InputAction secondaryAttack;

    [field: SerializeField] public Vector2 Aim { get; private set; }
    [field: SerializeField] public Vector2 MoveDir { get; private set; }
    [field: SerializeField] public bool IsJumping { get; private set; }
    [field: SerializeField] public bool IsDashing { get; private set; }
    [field: SerializeField] public bool IsInteracting { get; private set; }
    [field: SerializeField] public bool IsOpeningInventory { get; private set; }
    [field: SerializeField] public bool IsUsingPrimaryAttack { get; private set; }
    [field: SerializeField] public bool IsUsingSecondaryAttack { get; private set; }

    // Singleton
    public static PlayerControlManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("More than 1 PlayerControlManager. Destroying this. Name: " + name);
            return;
        }

        playerControls = new PlayerControls();

        aim = playerControls.Player.Aim;
        move = playerControls.Player.Move;
        jump = playerControls.Player.Jump;
        dash = playerControls.Player.Dash;
        interact = playerControls.Player.Interact;
        inventory = playerControls.Player.Inventory; 
        primaryAttack= playerControls.Player.PrimaryAttack;
        secondaryAttack= playerControls.Player.SecondaryAttack;
    }

    private void OnEnable()
    {
        aim.Enable();
        move.Enable();
        jump.Enable();
        dash.Enable();
        interact.Enable();
        inventory.Enable();
        primaryAttack.Enable();
        secondaryAttack.Enable();
    }

    private void OnDisable()
    {
        aim.Disable();
        move.Disable();
        jump.Disable();
        dash.Disable();
        interact.Disable();
        inventory.Disable();
        primaryAttack.Disable();
        secondaryAttack.Disable();
    }

    private void Update()
    {
        Aim = aim.ReadValue<Vector2>();
        MoveDir = move.ReadValue<Vector2>();
        IsJumping = jump.IsPressed(); // Check holding
        IsDashing = dash.WasPressedThisFrame();
        IsInteracting = interact.WasPressedThisFrame();
        IsOpeningInventory = inventory.WasPressedThisFrame();
        IsUsingPrimaryAttack = primaryAttack.WasPressedThisFrame();
        IsUsingSecondaryAttack = secondaryAttack.WasPressedThisFrame();
    }
}
