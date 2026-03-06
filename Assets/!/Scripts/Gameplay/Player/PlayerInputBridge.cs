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

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        walkAction = playerInput.actions["Walk"];
        attackAction = playerInput.actions["Attack"];
        aimAction = playerInput.actions["Aim"];
    }

    private void Start()
    {
        GetComponent<NetworkRunner>().ProvideInput = true;
    }

    void OnEnable()
    {
        moveAction.Enable();
        walkAction.Enable();
        attackAction.Enable();
        aimAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        walkAction.Disable();
        attackAction.Disable();
        aimAction.Disable();
    }

    public void OnFusionInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData
        {
            Move = moveAction.ReadValue<Vector2>(),
            Walk = walkAction.ReadValue<float>() > 0f,
            Attack = attackAction.IsPressed(),
            Aim = aimAction.IsPressed()
        };

        input.Set(data);
    }
}
