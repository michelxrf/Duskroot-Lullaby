using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoSelectFirstAvailableButton : MonoBehaviour
{
    [Header("Buttons To Check")]
    [SerializeField] private Button[] buttons;

    [Header("Settings")]
    [SerializeField] private bool autoValidate = true;

    private void Start()
    {
        SelectFirstAvailable();
    }

    private void Update()
    {
        if (!autoValidate)
            return;

        ValidateCurrentSelection();
    }

    /// <summary>
    /// Selects the first available button.
    /// </summary>
    public void SelectFirstAvailable()
    {
        foreach (Button button in buttons)
        {
            if (button == null)
                continue;

            if (!button.gameObject.activeInHierarchy)
                continue;

            if (!button.interactable)
                continue;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Checks if the currently selected button is still valid.
    /// If not, selects another one.
    /// </summary>
    public void ValidateCurrentSelection()
    {
        GameObject current =
            EventSystem.current.currentSelectedGameObject;

        if (current == null)
        {
            SelectFirstAvailable();
            return;
        }

        Button currentButton =
            current.GetComponent<Button>();

        if (currentButton == null)
        {
            SelectFirstAvailable();
            return;
        }

        if (!currentButton.interactable ||
            !currentButton.gameObject.activeInHierarchy)
        {
            SelectFirstAvailable();
        }
    }

}