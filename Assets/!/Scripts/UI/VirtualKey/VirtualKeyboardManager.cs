using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

public class VirtualKeyboardManager : MonoBehaviour
{
    public static VirtualKeyboardManager Instance;

    [Header("References")]
    [SerializeField] private GameObject keyboardCanvas;
    [SerializeField] private GameObject firstKey;

    [Header("Caps Lock UI")]
    [SerializeField] private Transform capsLockVisual;
    [SerializeField] private GameObject arrowUp;
    [SerializeField] private GameObject arrowDown;

    [SerializeField] private GameObject notificationObject;
    [SerializeField] private TMP_Text notificationText;

    private TMP_InputField currentInputField;
    private GameObject nextSelected;
    private Coroutine notificationRoutine;
    private bool capsLockEnabled = false;
    private GameObject previousSelected;

    public bool IsOpen => keyboardCanvas.activeSelf;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (!IsOpen)
            return;

        HandleControllerShortcuts();
    }

    public void OpenKeyboard(
        TMP_InputField inputField,
        GameObject nextSelectable)
    {
        previousSelected =
            EventSystem.current.currentSelectedGameObject;

        currentInputField = inputField;
        nextSelected = nextSelectable;

        keyboardCanvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstKey);
    }

    public void AddCharacter(string character)
    {
        if (currentInputField == null)
            return;

        bool shouldUppercase =
            capsLockEnabled ||
            currentInputField.text.Length == 0 ||
            currentInputField.text.EndsWith(" ");

        currentInputField.text +=
            shouldUppercase
            ? character.ToUpper()
            : character.ToLower();
    }

    public void AddSpace()
    {
        if (currentInputField == null)
            return;

        currentInputField.text += " ";
    }

    public void DeleteLastCharacter()
    {
        if (currentInputField == null)
            return;

        string text = currentInputField.text;

        if (text.Length <= 0)
            return;

        currentInputField.text =
            text.Substring(0, text.Length - 1);
    }

    public void Confirm()
    {
        keyboardCanvas.SetActive(false);

        EventSystem.current
            .SetSelectedGameObject(null);

        EventSystem.current
            .SetSelectedGameObject(nextSelected);

        currentInputField = null;
    }

    public void ToggleCapsLock()
    {
        capsLockEnabled =
            !capsLockEnabled;

        UpdateCapsVisual();

        ShowCapsMessage(
            capsLockEnabled
            ? "Caps Lock ON"
            : "Caps Lock OFF");
    }

    private void UpdateCapsVisual()
    {
        if (capsLockEnabled)
        {
            capsLockVisual.localScale =
                Vector3.one * 1.1f;

            arrowUp.SetActive(true);
            arrowDown.SetActive(false);
        }
        else
        {
            capsLockVisual.localScale =
                Vector3.one;

            arrowUp.SetActive(false);
            arrowDown.SetActive(true);
        }
    }

    private void ShowCapsMessage(string message)
    {
        if (notificationRoutine != null)
            StopCoroutine(notificationRoutine);

        notificationRoutine =
            StartCoroutine(
                ShowCapsMessageRoutine(message));
    }

    private IEnumerator ShowCapsMessageRoutine(
    string message)
    {
        notificationObject.SetActive(true);

        notificationText.text = message;

        yield return new WaitForSeconds(1f);

        notificationObject.SetActive(false);
    }

    private void HandleControllerShortcuts()
    {
        Gamepad gamepad = Gamepad.current;

        if (gamepad == null)
            return;

        // X
        if (gamepad.buttonWest.wasPressedThisFrame)
            DeleteLastCharacter();
        // Y
        if (gamepad.buttonNorth.wasPressedThisFrame)
            AddSpace();
        // RB
        if (gamepad.rightShoulder.wasPressedThisFrame)
            ToggleCapsLock();
        // RT
        if (gamepad.rightTrigger.wasPressedThisFrame)
            Confirm();
        // B
        if (gamepad.buttonEast.wasPressedThisFrame)
            CancelKeyboard();
    }

    public void CancelKeyboard()
    {
        keyboardCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current
            .SetSelectedGameObject(previousSelected);
    }
}