using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualKeyboardManager : MonoBehaviour
{
    public static VirtualKeyboardManager Instance;

    [Header("References")]
    [SerializeField] private GameObject keyboardCanvas;
    [SerializeField] private GameObject firstKey;

    private TMP_InputField currentInputField;
    private GameObject nextSelected;

    public bool IsOpen => keyboardCanvas.activeSelf;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenKeyboard(
        TMP_InputField inputField,
        GameObject nextSelectable)
    {
        currentInputField = inputField;
        nextSelected = nextSelectable;

        keyboardCanvas.SetActive(true);

        EventSystem.current
            .SetSelectedGameObject(null);

        EventSystem.current
            .SetSelectedGameObject(firstKey);
    }

    public void AddCharacter(string character)
    {
        if (currentInputField == null)
            return;

        currentInputField.text += character;
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
}