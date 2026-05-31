using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    public static InputDeviceManager Instance;

    public Action<InputDeviceType> OnDeviceChanged;

    public InputDeviceType CurrentDevice
    {
        get;
        private set;
    }

    [Header("Detection")]
    [SerializeField] private float stickDeadzone = 0.2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        DetectKeyboardMouse();
        DetectGamepad();
    }

    private void DetectKeyboardMouse()
    {
        // Keyboard
        if (Keyboard.current != null &&
            Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SetDevice(InputDeviceType.KeyboardMouse);
            return;
        }

        // Mouse movement
        if (Mouse.current != null)
        {
            Vector2 mouseDelta =
                Mouse.current.delta.ReadValue();

            if (mouseDelta.sqrMagnitude > 0.01f)
            {
                SetDevice(InputDeviceType.KeyboardMouse);
                return;
            }

            // Mouse click
            if (Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame)
            {
                SetDevice(InputDeviceType.KeyboardMouse);
            }
        }
    }

    private void DetectGamepad()
    {
        if (Gamepad.current == null)
            return;

        Gamepad gamepad = Gamepad.current;

        // Left Stick
        if (gamepad.leftStick.ReadValue().magnitude > stickDeadzone)
        {
            SetDevice(DetectDevice(gamepad));
            return;
        }

        // Right Stick
        if (gamepad.rightStick.ReadValue().magnitude > stickDeadzone)
        {
            SetDevice(DetectDevice(gamepad));
            return;
        }

        // DPad
        if (gamepad.dpad.up.wasPressedThisFrame ||
            gamepad.dpad.down.wasPressedThisFrame ||
            gamepad.dpad.left.wasPressedThisFrame ||
            gamepad.dpad.right.wasPressedThisFrame)
        {
            SetDevice(DetectDevice(gamepad));
            return;
        }

        // Face Buttons
        if (gamepad.buttonSouth.wasPressedThisFrame ||
            gamepad.buttonNorth.wasPressedThisFrame ||
            gamepad.buttonEast.wasPressedThisFrame ||
            gamepad.buttonWest.wasPressedThisFrame)
        {
            SetDevice(DetectDevice(gamepad));
            return;
        }

        // Shoulder Buttons
        if (gamepad.leftShoulder.wasPressedThisFrame ||
            gamepad.rightShoulder.wasPressedThisFrame)
        {
            SetDevice(DetectDevice(gamepad));
            return;
        }

        // Triggers
        if (gamepad.leftTrigger.ReadValue() > stickDeadzone || gamepad.rightTrigger.ReadValue() > stickDeadzone)
        {
            SetDevice(DetectDevice(gamepad));
        }
    }

    private void SetDevice(InputDeviceType newDevice)
    {
        if (newDevice == CurrentDevice)
            return;

        CurrentDevice = newDevice;

        //Debug.Log($"Input changed to: {CurrentDevice}");

        OnDeviceChanged?.Invoke(CurrentDevice);
    }

    private InputDeviceType DetectDevice(InputDevice device)
    {
        if (device is Keyboard || device is Mouse)
        {
            return InputDeviceType.KeyboardMouse;
        }

        if (device is Gamepad)
        {
            string deviceName =
                device.displayName.ToLower();

            //Debug.Log($"Detected device: {deviceName}");

            // PlayStation
            if (deviceName.Contains("dualsense") ||
                deviceName.Contains("dualshock") ||
                deviceName.Contains("playstation") ||
                deviceName.Contains("wireless controller") ||
                deviceName.Contains("ps4") ||
                deviceName.Contains("ps5"))
            {
                return InputDeviceType.PlayStation;
            }

            // Nintendo Switch
            if (deviceName.Contains("nintendo") ||
                deviceName.Contains("switch") ||
                deviceName.Contains("joy-con") ||
                deviceName.Contains("pro controller"))
            {
                return InputDeviceType.NintendoSwitch;
            }

            // Fallback
            return InputDeviceType.Xbox;
        }

        return InputDeviceType.KeyboardMouse;
    }
}