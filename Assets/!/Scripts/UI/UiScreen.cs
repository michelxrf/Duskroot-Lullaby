using UnityEngine;

/// <summary>
/// Handles the visibility of a UI screen in the game.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UiScreen : MonoBehaviour
{
    [SerializeField] protected UiManager uiManager;
    [SerializeField] bool showOnStart = false;

    protected virtual void Start()
    {
        if (showOnStart)
            uiManager.ShowScreen(this);
        else
            Hide();
    }

    public virtual void Show()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void Hide()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
