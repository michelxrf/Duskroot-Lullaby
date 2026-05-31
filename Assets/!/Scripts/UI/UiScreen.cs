using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UiScreen : MonoBehaviour
{
    [SerializeField]
    protected UiManager uiManager;

    [SerializeField]
    bool showOnStart = false;

    MenuScreen menuScreen;

    protected virtual void Start()
    {
        menuScreen =
            GetComponent<MenuScreen>();

        if (showOnStart)
            uiManager.ShowScreen(this);
        else
            Hide();
    }

    public virtual void Show()
    {
        var canvasGroup =
            GetComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // seleciona botão da tela aberta
        menuScreen?.SelectFirstButton();
    }

    public virtual void Hide()
    {
        var canvasGroup =
            GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}