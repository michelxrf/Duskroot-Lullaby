using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;

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
    /// <summary>Tooltip UI element displayed when a player is in interaction range.</summary>
    [SerializeField] GameObject interactionTooltip;
    /// <summary>UI balloon/panel that displays dialogue text during bark sequences.</summary>
    [SerializeField] GameObject BarkBalloon;
    /// <summary>TextMeshPro text component for displaying bark dialogue text.</summary>
    [SerializeField] TMP_Text barkTextField;
    /// <summary>Array of bark dialogue sequences to play when activated.</summary>
    [SerializeField] Bark[] barks;
    [SerializeField] private FMODUnity.EventReference barkEvent;

    [Header("Settings")]
    /// <summary>Radius of the sphere collider for detecting nearby players.</summary>
    [SerializeField] float interactionRange = 3f;
    /// <summary>If true, the interaction disables itself after playing once.</summary>
    [SerializeField] bool singleActivation = false;
    /// <summary>Reference to an alternative Interactions component to switch to after interaction.</summary>
    [SerializeField] Interactions changeToAfterInteraction;
    /// <summary>Delay in seconds between successive barks in a sequence.</summary>
    [SerializeField] float delayBetweenBarks = 1f;

    /// <summary>Networked flag indicating whether a bark sequence is currently playing.</summary>
    [Networked] bool isInteracting { get; set; }
    /// <summary>Number of players currently within the interaction range.</summary>
    int playersInRange;
    /// <summary>Audio source component for playing bark audio clips.</summary>
    AudioSource audioSource;
    /// <summary>Flag to ensure initialization only occurs after the object has spawned on the network.</summary>
    bool hasSpawned = false;

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

        if (!isInteracting && playersInRange >= 1)
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

        if (playersInRange < 1 || isInteracting || barks.Length == 0)
            return;

        Debug.Log("Activating Bark Sequence");

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
        for (int i = 0; i < barks.Length; i++)
        {
            Bark bark = barks[i];
            barkTextField.text = bark.text;

            if(!barkEvent.IsNull)
{
                Debug.Log($"Playing FMOD audio for bark {i}");

                var instance = FMODUnity.RuntimeManager.CreateInstance(barkEvent);
                instance.setParameterByName("BarkNumber", bark.barkNumber); // escolhe qual fala (1 a 3 para esse teste)
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform)); //Usado para setar o local onde o som será emitido (3D)
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

        Debug.Log("Bark Sequence Completed");
    }
}
