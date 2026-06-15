using CombatSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

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
    private PlayerSetup localPlayer;
    private PlayerHealth localPlayerHealth;

    private bool isReading = false;
    private int openFrame = -1;

    public static List<ReadableCard> OpenCards = new List<ReadableCard>();

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
        if (cardUI == null)
            return;

        localPlayer = FindFirstObjectByType<PlayerSetup>();

        if (localPlayer != null &&
            localPlayer.IsLocalPlayer())
        {
            localPlayer.EnablePlayerControlsLocal(false);

            Animator animator =
                localPlayer.GetComponentInChildren<Animator>();

            if (animator != null)
                animator.SetFloat("Speed", 0);

            localPlayerHealth =
                localPlayer.GetComponent<PlayerHealth>();

            if (localPlayerHealth != null)
                localPlayerHealth.IsInvulnerable = true;
        }

        isReading = true;
        openFrame = Time.frameCount;

        cardUI.SetActive(true);
        cardText.text = cardContent;

        if (interactionTooltip != null)
            interactionTooltip.SetActive(false);

        if (!OpenCards.Contains(this))
            OpenCards.Add(this);
    }

    public void CloseCard()
    {
        if (cardUI == null)
            return;

        OpenCards.Remove(this);

        if (cardUI == null)
            return;

        if (localPlayer != null)
        {
            if (localPlayerHealth != null &&
                !localPlayerHealth.IsDead())
            {
                localPlayerHealth.IsInvulnerable = false;
                localPlayer.EnablePlayerControlsLocal(true);
            }
        }

        isReading = false;
        cardUI.SetActive(false);
    }

    public static void CloseAllCards()
    {
        for (int i = OpenCards.Count - 1; i >= 0; i--)
        {
            if (OpenCards[i] != null)
                OpenCards[i].CloseCard();
        }

        OpenCards.Clear();
    }
}
