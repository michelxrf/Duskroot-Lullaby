using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages NPC interactions and dialogue (bark) sequences in a networked environment.
/// Handles displaying tooltips when players are in range, activating bark sequences,
/// and managing audio playback for dialogue.
/// </summary>
public class Interactions : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] GameObject interactionTooltip;
    [SerializeField] GameObject BarkBalloon;
    [SerializeField] TMP_Text barkTextField;
    [SerializeField] Bark[] barks;
    [SerializeField] private FMODUnity.EventReference barkEvent;
    [SerializeField] private string audioBarkParam = "BarkNumber";

    [Header("Settings")]
    [SerializeField] float interactionRange = 3f;
    [SerializeField] bool canInteract = true;
    [SerializeField] bool singleActivation = false;
    [SerializeField] Interactions changeToAfterInteraction;
    [SerializeField] float delayBetweenBarks = 1f;

    // Use OnChangedRender to automatically sync and trigger the UI on all clients
    [Networked, OnChangedRender(nameof(OnInteractingChanged))]
    bool isInteracting { get; set; }

    private Coroutine barkCoroutine;
    AudioSource audioSource;
    bool isPlayerClose = false;
    bool hasSpawned = false;

    [SerializeField] private UnityEvent OnSequenceEnded;
    [SerializeField] private UnityEvent OnSequenceStarted;

    /// <summary>
    /// Called when the object spawns on the network.
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();
        hasSpawned = true;

        GetComponent<SphereCollider>().radius = interactionRange;

        BarkBalloon.SetActive(false);
        interactionTooltip.SetActive(false);
    }

    /// <summary>
    /// Property changed callback triggered on all clients when isInteracting changes.
    /// </summary>
    public void OnInteractingChanged()
    {
        if (isInteracting)
        {
            if (barkCoroutine != null) StopCoroutine(barkCoroutine);
            barkCoroutine = StartCoroutine(PlayBarkSequence());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        isPlayerClose = true;
        other.GetComponent<PlayerInteractor>()?.EnteredInteractionArea(this);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        isPlayerClose = false;
        other.GetComponent<PlayerInteractor>()?.LeftInteractionArea();
    }

    void CanInteract()
    {
        if (!hasSpawned)
            return;

        if (canInteract && !isInteracting && isPlayerClose)
        {
            interactionTooltip.SetActive(true);
        }
        else
        {
            interactionTooltip.SetActive(false);
        }
    }

    private void Update()
    {
        CanInteract();
    }

    /// <summary>
    /// RPC method to initiate a bark dialogue sequence.
    /// Executed ONLY on the State Authority to safely modify the networked state.
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ActivateBark()
    {
        Debug.Log("RPC_ActivateBark called on State Authority");

        if (!canInteract || isInteracting || barks.Length == 0)
            return;

        isInteracting = true;
    }

    /// <summary>
    /// Coroutine that plays through the bark sequence locally on each client.
    /// </summary>
    IEnumerator PlayBarkSequence()
    {
        OnSequenceStarted?.Invoke();
        BarkBalloon.SetActive(true);
        interactionTooltip.SetActive(false);

        for (int i = 0; i < barks.Length; i++)
        {
            Bark bark = barks[i];
            barkTextField.text = bark.text;

            if (!barkEvent.IsNull)
            {
                var instance = FMODUnity.RuntimeManager.CreateInstance(barkEvent);
                instance.setParameterByName(audioBarkParam, bark.barkNumber);
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
                instance.start();
                FMOD.Studio.PLAYBACK_STATE state;
                do
                {
                    instance.getPlaybackState(out state);
                    yield return null;
                }
                while (state != FMOD.Studio.PLAYBACK_STATE.STOPPED);
                yield return new WaitForSeconds(delayBetweenBarks);
                instance.release();
            }
        }

        BarkBalloon.SetActive(false);

        // Only the state authority resets the networked state to false
        if (HasStateAuthority)
        {
            isInteracting = false;
        }

        // Disable this interaction locally on each client if configured for single use
        if (singleActivation)
        {
            enabled = false;
        }

        OnSequenceEnded?.Invoke();
        Debug.Log("Bark Sequence Completed");
    }
}