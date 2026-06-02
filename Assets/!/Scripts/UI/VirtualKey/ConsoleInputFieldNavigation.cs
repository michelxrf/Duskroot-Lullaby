using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConsoleInputFieldNavigation :
    MonoBehaviour,
    ISelectHandler
{
    private TMP_InputField inputField;

    private void Awake()
    {
        inputField =
            GetComponent<TMP_InputField>();
    }

    public void OnSelect(
        BaseEventData eventData)
    {
        TryDisableEditMode();
    }

    private void Update()
    {
        bool isSelected =
            EventSystem.current
            .currentSelectedGameObject
            == gameObject;

        if (!isSelected)
            return;

        TryDisableEditMode();
    }

    private void TryDisableEditMode()
    {
        if (InputDeviceManager.Instance == null)
            return;

        // Mouse + Keyboard => comportamento normal
        if (InputDeviceManager.Instance.CurrentDevice ==
            InputDeviceType.KeyboardMouse)
        {
            return;
        }

        // Console => impedir edit mode
        if (inputField.isFocused)
        {
            inputField.DeactivateInputField();
        }
    }
}