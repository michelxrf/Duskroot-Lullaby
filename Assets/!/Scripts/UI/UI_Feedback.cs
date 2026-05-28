using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Feedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("HOVER CONFIG")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float moveRightAmount = 20f;
    [SerializeField] private float animationSpeed = 10f;

    private RectTransform rectTransform;

    private Vector3 originalScale;
    private Vector2 originalPosition;

    private Vector3 targetScale;
    private Vector2 targetPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;

        targetScale = originalScale;
        targetPosition = originalPosition;
    }

    private void Update()
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.deltaTime * animationSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
        targetPosition = originalPosition + Vector2.right * moveRightAmount;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        targetPosition = originalPosition;
    }
}