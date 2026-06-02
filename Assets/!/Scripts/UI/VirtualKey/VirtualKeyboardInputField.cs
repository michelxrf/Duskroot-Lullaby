using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class VirtualKeyboardInputField :
    MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField]
    private GameObject nextSelectable;

    private TMP_InputField inputField;

    private void Awake()
    {
        inputField =
            GetComponent<TMP_InputField>();
    }

    private void Update()
    {
        if (EventSystem.current
            .currentSelectedGameObject != gameObject)
            return;

        if (InputDeviceManager.Instance
            .CurrentDevice ==
            InputDeviceType.KeyboardMouse)
            return;

        if (Gamepad.current == null)
            return;

        if (Gamepad.current.buttonSouth
            .wasPressedThisFrame)
        {
            VirtualKeyboardManager.Instance
                .OpenKeyboard(
                    inputField,
                    nextSelectable
                );
        }
    }
}