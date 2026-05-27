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
/// 
//[RequireComponent(typeof(AudioSource))]
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

    [Networked] bool isInteracting { get; set; }
    int playersInRange;
    AudioSource audioSource;
    bool hasSpawned = false;

    [SerializeField] private UnityEvent OnSequenceEnded;
    [SerializeField] private UnityEvent OnSequenceStarted;

    /// <summary>
    /// Called when the object spawns on the network. Initializes interaction range,
    /// audio source, and UI elements.
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();
        hasSpawned = true;

        playersInRange = 0;
        isInteracting = false;

        GetComponent<SphereCollider>().radius = interactionRange;

        BarkBalloon.SetActive(false);
        interactionTooltip.SetActive(false);
    }

    /// <summary>
    /// Called when a player enters the interaction trigger zone.
    /// Increments player count and notifies the player of the interaction area.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerSetup>() == null)
            return;

        playersInRange++;
        other.GetComponent<PlayerInteractor>()?.EnteredInteractionArea(this);
    }

    /// <summary>
    /// Called when a player exits the interaction trigger zone.
    /// Decrements player count and notifies the player they've left the interaction area.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerSetup>() == null)
            return;

        playersInRange--;
        other.GetComponent<PlayerInteractor>()?.LeftInteractionArea();
    }

    /// <summary>
    /// Determines whether the interaction tooltip should be visible.
    /// Shows tooltip if a player is in range and no bark sequence is currently playing.
    /// </summary>
    void CanInteract()
    {
        if (!hasSpawned)
            return;

        if (canInteract && !isInteracting && playersInRange >= 1)
        {
            interactionTooltip.SetActive(true);
        }
        else
        {
            interactionTooltip.SetActive(false);
        }
    }

    /// <summary>
    /// Called every frame to update the visibility of the interaction tooltip.
    /// </summary>
    private void Update()
    {
        CanInteract();
    }

    /// <summary>
    /// RPC method to initiate a bark dialogue sequence.
    /// Validates that a player is present, no sequence is already playing, and barks exist.
    /// Called by all clients and executed on all clients to synchronize the bark sequence.
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ActivateBark()
    {
        Debug.Log("RPC_ActivateBark called");

        if (!canInteract || playersInRange < 1 || isInteracting || barks.Length == 0)
            return;

        isInteracting = true;
        interactionTooltip.SetActive(false);
        BarkBalloon.SetActive(true);
        StartCoroutine(PlayBarkSequence());
    }

    /// <summary>
    /// Coroutine that plays through the bark sequence.
    /// Displays text and plays audio for each bark, with delays between them.
    /// Hides the dialogue balloon and marks interaction as complete when finished.
    /// </summary>
    IEnumerator PlayBarkSequence()
    {
        OnSequenceStarted?.Invoke();

        for (int i = 0; i < barks.Length; i++)
        {
            Bark bark = barks[i];
            barkTextField.text = bark.text;

            if(!barkEvent.IsNull)
{
                Debug.Log($"Playing FMOD audio for bark {i} param"+ audioBarkParam);

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
        isInteracting = false;

        // Disable this interaction if it's configured for single use
        if (singleActivation)
        {
            enabled = false;
        }

        OnSequenceEnded?.Invoke();
        Debug.Log("Bark Sequence Completed");
    }
}
