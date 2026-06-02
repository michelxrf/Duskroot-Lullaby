using UnityEngine;
using UnityEngine.UI;

public class UIInputIcon : MonoBehaviour
{
    [SerializeField]
    private InputIconDatabase iconDatabase;

    [SerializeField]
    private InputActionIcon action;

    private Image image;
    private RectTransform rectTransform;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        InputDeviceManager.Instance
            .OnDeviceChanged += UpdateIcon;

        UpdateIcon(
            InputDeviceManager.Instance
            .CurrentDevice);
    }

    private void OnDisable()
    {
        if (InputDeviceManager.Instance == null)
            return;

        InputDeviceManager.Instance
            .OnDeviceChanged -= UpdateIcon;
    }

    private void UpdateIcon(
        InputDeviceType device)
    {
        var icon =
            iconDatabase.GetIcon(action, device);

        if (icon == null)
            return;

        image.sprite = icon.sprite;

        rectTransform.sizeDelta =
            icon.size;
    }
}