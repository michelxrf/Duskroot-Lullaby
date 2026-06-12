using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Handles interaction with readable cards, displaying a world-space tooltip 
/// and a screen-space UI when interacted with.
/// </summary>
public class ReadableCard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionTooltip;
    [SerializeField] private GameObject cardUI;
    [SerializeField] private TMP_Text cardText;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] [TextArea(3, 10)] private string cardContent;

    private bool isReading = false;
    private int openFrame = -1;

    private void Start()
    {
        if (interactionTooltip != null)
            interactionTooltip.SetActive(false);
            
        if (cardUI != null)
            cardUI.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCard);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        interactionTooltip.SetActive(true);
        other.GetComponent<PlayerInteractor>()?.EnteredReadableCardArea(this);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        interactionTooltip.SetActive(false);
        other.GetComponent<PlayerInteractor>()?.LeftReadableCardArea(this);    
        CloseCard();     
    }

    private void Update()
    {
        if (isReading && Time.frameCount > openFrame)
        {
            // Close on any button press (including movement WASD)
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                CloseCard();
            }
            else if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
            {
                CloseCard();
            }
        }
    }

    public void OpenCard()
    {
        if (cardUI == null) return;

        isReading = true;
        openFrame = Time.frameCount;
        cardUI.SetActive(true);
        if (cardText != null)
        {
            cardText.text = cardContent;
        }
        
        // Ensure tooltip is hidden while reading
        if (interactionTooltip != null)
            interactionTooltip.SetActive(false);
    }

    public void CloseCard()
    {
        if (cardUI == null) return;

        isReading = false;
        cardUI.SetActive(false);
    }
}
