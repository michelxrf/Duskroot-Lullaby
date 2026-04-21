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
    InputAction debugReviveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        walkAction = playerInput.actions["Walk"];
        attackAction = playerInput.actions["Attack"];
        aimAction = playerInput.actions["Aim"];
        interactAciton = playerInput.actions["Interact"];
        debugReviveAction = playerInput.actions["DebugRevive"];
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
        debugReviveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        walkAction.Disable();
        attackAction.Disable();
        aimAction.Disable();
        interactAciton.Disable();
        debugReviveAction.Disable();
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
            DebugRevive = debugReviveAction.IsPressed()
        };

        input.Set(data);
    }

    private void OnDestroy()
    {
        RunnerBootstrap.Instance.OnInput -= OnFusionInput;
    }
}
