using CombatSystem;
using Fusion;
using UnityEngine;

/// <summary>
/// Handles player emotes by triggering animations based on network input.
/// Syncs triggers across the network using RPCs and follows the project's input patterns.
/// </summary>
public class EmoteSystem : NetworkBehaviour
{
    private Animator animator;
    private PlayerHealth playerHealth;
    private NetworkInputData lastData;
    private bool isEmotePending;

    public override void Spawned()
    {
        animator = GetComponentInChildren<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public override void FixedUpdateNetwork()
    {
        // Only the State Authority (owner) should process input and send commands
        if (!HasStateAuthority)
            return;

        if (playerHealth.IsDead() || playerHealth.IsInvulnerable)
            return;

        if (GetInput(out NetworkInputData data))
        {
            // If any movement or combat input is received, clear pending emote triggers
            if (AnyOtherInput(data))
            {
                if (isEmotePending)
                {
                    RPC_ClearEmotes();
                    isEmotePending = false;
                }
            }
            else
            {
                // Trigger emotes based on input data (detecting new presses)
                if (data.Emote0 && !lastData.Emote0) { RPC_PlayEmote("Emote0"); isEmotePending = true; }
                else if (data.Emote1 && !lastData.Emote1) { RPC_PlayEmote("Emote1"); isEmotePending = true; }
                else if (data.Emote2 && !lastData.Emote2) { RPC_PlayEmote("Emote2"); isEmotePending = true; }
                else if (data.Emote3 && !lastData.Emote3) { RPC_PlayEmote("Emote3"); isEmotePending = true; }
            }

            lastData = data;
        }
    }

    /// <summary>
    /// Checks if any input other than emotes is being received.
    /// </summary>
    private bool AnyOtherInput(NetworkInputData data)
    {
        // Based on ReadableCard and PlayerMovement logic, we check for movement and actions
        return data.Move.magnitude > 0.1f || 
               data.Attack || 
               data.Dash || 
               data.Interact || 
               data.Walk || 
               data.Aim ||
               data.Skill1 ||
               data.Skill2;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayEmote(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ClearEmotes()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Emote0");
            animator.ResetTrigger("Emote1");
            animator.ResetTrigger("Emote2");
            animator.ResetTrigger("Emote3");
        }
    }
}
