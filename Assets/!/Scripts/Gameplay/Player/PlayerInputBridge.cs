using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Provides a bridge between Unity's PlayerInput system and Fusion networking by translating local player input into
/// networked input data.
/// </summary>
public class PlayerInputBridge : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction walkAction;
    InputAction attackAction;
    InputAction aimAction;
    InputAction interactAciton;
    InputAction dashAction;
    InputAction debugReviveAction;
    InputAction lookAction;
    InputAction emote0;
    InputAction emote1;
    InputAction emote2;
    InputAction emote3;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        walkAction = playerInput.actions["Walk"];
        attackAction = playerInput.actions["Attack"];
        aimAction = playerInput.actions["Aim"];
        interactAciton = playerInput.actions["Interact"];
        dashAction = playerInput.actions["Dash"];
        debugReviveAction = playerInput.actions["DebugRevive"];
        lookAction = playerInput.actions["Look"];
        emote0 = playerInput.actions["Emote0"];
        emote1 = playerInput.actions["Emote1"];
        emote2 = playerInput.actions["Emote2"];
        emote3 = playerInput.actions["Emote3"];
    }

    private void Start()
    {
        GetComponent<NetworkRunner>().ProvideInput = true;
        RunnerBootstrap.Instance.OnInput += OnFusionInput;
    }

    void OnEnable()
    {
        moveAction.Enable();
        walkAction.Enable();
        attackAction.Enable();
        aimAction.Enable();
        interactAciton.Enable();
        dashAction.Enable();
        debugReviveAction.Enable();
        lookAction.Enable();
        emote0.Enable();
        emote1.Enable();
        emote2.Enable();
        emote3.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        walkAction.Disable();
        attackAction.Disable();
        aimAction.Disable();
        interactAciton.Disable();
        dashAction.Disable();
        debugReviveAction.Disable();
        lookAction.Disable();
        emote0.Disable();
        emote1.Disable();
        emote2.Disable();
        emote3.Disable();
    }

    public void OnFusionInput(NetworkRunner runner, NetworkInput input)
    {


        NetworkInputData data = new NetworkInputData
        {
            Move = moveAction.ReadValue<Vector2>(),
            Walk = walkAction.ReadValue<float>() > 0f,
            Attack = attackAction.IsPressed(),
            Aim = aimAction.IsPressed(),
            Interact = interactAciton.IsPressed(),
            Dash = dashAction.IsPressed(),
            DebugRevive = debugReviveAction.IsPressed(),
            Look = lookAction.ReadValue<Vector2>(),
            Emote0 = emote0.IsPressed(),
            Emote1 = emote1.IsPressed(),
            Emote2 = emote2.IsPressed(),
            Emote3 = emote3.IsPressed()
        };

        input.Set(data);
    }

    private void OnDestroy()
    {
        RunnerBootstrap.Instance.OnInput -= OnFusionInput;
    }
}
