using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputFieldSelectedVisual :
    MonoBehaviour,
    ISelectHandler,
    IDeselectHandler
{
    [Header("References")]
    [SerializeField]
    private GameObject selectedImage;

    private TMP_InputField inputField;

    private void Awake()
    {
        inputField =
            GetComponent<TMP_InputField>();

        if (selectedImage != null)
        {
            selectedImage.SetActive(false);
        }
    }

    public void OnSelect(
        BaseEventData eventData)
    {
        if (selectedImage == null)
            return;

        selectedImage.SetActive(true);
    }

    public void OnDeselect(
        BaseEventData eventData)
    {
        if (selectedImage == null)
            return;

        selectedImage.SetActive(false);
    }
}