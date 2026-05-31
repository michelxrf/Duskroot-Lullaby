using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_FeedbackProxy : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Button sourceButton;
    [SerializeField] private RectTransform feedbackTarget;
    [SerializeField] private Image targetImage;

    [Header("Hover Config")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float moveRightAmount = 20f;
    [SerializeField] private float animationSpeed = 10f;

    private Vector3 originalScale;
    private Vector2 originalPosition;

    private Vector3 targetScale;
    private Vector2 targetPosition;

    private Color currentTargetColor;

    private void Awake()
    {
        if (sourceButton == null)
        {
            sourceButton =
                GetComponent<Button>();
        }

        originalScale =
            feedbackTarget.localScale;

        originalPosition =
            feedbackTarget.anchoredPosition;

        targetScale =
            originalScale;

        targetPosition =
            originalPosition;

        currentTargetColor =
            GetMultipliedColor(
                sourceButton.colors.normalColor
            );

        targetImage.color =
            currentTargetColor;
    }

    private void Update()
    {
        AnimateTransform();
        AnimateColor();
        UpdateDisabledState();
    }

    private void AnimateTransform()
    {
        feedbackTarget.localScale =
            Vector3.Lerp(
                feedbackTarget.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );

        feedbackTarget.anchoredPosition =
            Vector2.Lerp(
                feedbackTarget.anchoredPosition,
                targetPosition,
                Time.deltaTime * animationSpeed
            );
    }

    private void AnimateColor()
    {
        float fadeDuration =
            sourceButton.colors.fadeDuration;

        float lerpSpeed =
            fadeDuration <= 0
            ? 999f
            : 1f / fadeDuration;

        targetImage.color =
            Color.Lerp(
                targetImage.color,
                currentTargetColor,
                Time.deltaTime * lerpSpeed
            );
    }

    private void UpdateDisabledState()
    {
        if (!sourceButton.interactable)
        {
            currentTargetColor =
                GetMultipliedColor(
                    sourceButton.colors.disabledColor
                );
        }
    }

    private Color GetMultipliedColor(
        Color baseColor)
    {
        return baseColor *
               sourceButton.colors
               .colorMultiplier;
    }

    private void ApplyHover()
    {
        targetScale =
            originalScale *
            hoverScale;

        targetPosition =
            originalPosition +
            Vector2.right *
            moveRightAmount;
    }

    private void RemoveHover()
    {
        targetScale =
            originalScale;

        targetPosition =
            originalPosition;
    }

    private void SetButtonColor(
        Color color)
    {
        currentTargetColor =
            GetMultipliedColor(
                color
            );
    }

    // Mouse Hover

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        ApplyHover();

        SetButtonColor(
            sourceButton.colors
            .highlightedColor
        );
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        RemoveHover();

        SetButtonColor(
            sourceButton.colors
            .normalColor
        );
    }

    // Keyboard / Controller

    public void OnSelect(
        BaseEventData eventData)
    {
        ApplyHover();

        SetButtonColor(
            sourceButton.colors
            .selectedColor
        );
    }

    public void OnDeselect(
        BaseEventData eventData)
    {
        RemoveHover();

        SetButtonColor(
            sourceButton.colors
            .normalColor
        );
    }

    // Pressed

    public void OnPointerDown(
        PointerEventData eventData)
    {
        SetButtonColor(
            sourceButton.colors
            .pressedColor
        );
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        bool selected =
            EventSystem.current
            .currentSelectedGameObject
            == gameObject;

        SetButtonColor(
            selected
            ? sourceButton.colors
                .selectedColor
            : sourceButton.colors
                .highlightedColor
        );
    }
}