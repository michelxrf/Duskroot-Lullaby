using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool MenuOpenCloseInput { get; private set; }
    private PlayerInput playerInput;
    private InputAction menuOpenCloseAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        playerInput = GetComponent<PlayerInput>();
        menuOpenCloseAction = playerInput.actions["MenuOpenClose"];
    }

    private void Update()
    {
        MenuOpenCloseInput = menuOpenCloseAction.WasPressedThisFrame();
    }
}
