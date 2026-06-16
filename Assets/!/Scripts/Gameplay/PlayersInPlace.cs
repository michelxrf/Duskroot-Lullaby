using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayersInPlace : NetworkBehaviour
{
    [SerializeField] private UnityEvent onAllPlayersInPlace = new UnityEvent();
    [SerializeField] private string[] requiredKeys;

    public string[] RequiredKeys => requiredKeys;

    private List<PlayerKeyInventory> inventoriesInTrigger = new List<PlayerKeyInventory>();

    private void OnTriggerEnter(Collider collision)
    {
        PlayerKeyInventory inventory = collision.GetComponent<PlayerKeyInventory>();
        if (inventory == null)
            return;
        
        if (!inventoriesInTrigger.Contains(inventory))
            inventoriesInTrigger.Add(inventory);

        CheckAllPlayersInPlace();
    }

    private void OnTriggerExit(Collider collision)
    {
        PlayerKeyInventory inventory = collision.GetComponent<PlayerKeyInventory>();
        if (inventory == null)
            return;

        inventoriesInTrigger.Remove(inventory);
        CheckAllPlayersInPlace();
    }

    private void CheckAllPlayersInPlace()
    {
        // 1. Check if all active players are in the trigger
        if (inventoriesInTrigger.Count != Runner.ActivePlayers.Count())
            return;

        // 2. Check keys if any are required
        if (requiredKeys != null && requiredKeys.Length > 0)
        {
            // Collect all keys currently held by all players in the trigger
            HashSet<string> collectiveKeys = new HashSet<string>();
            foreach (var inv in inventoriesInTrigger)
            {
                List<string> playerKeys = inv.GetKeys();
                if (playerKeys != null)
                {
                    foreach (var key in playerKeys)
                    {
                        collectiveKeys.Add(key);
                    }
                }
            }

            // Verify each required key is present in the collective set
            foreach (var req in requiredKeys)
            {
                if (string.IsNullOrEmpty(req)) continue;
                
                if (!collectiveKeys.Contains(req))
                {
                    return; // Requirement not met
                }
            }
        }

        // All conditions met: all players present and group has all required keys
        onAllPlayersInPlace?.Invoke();
        RPC_RemoveKeysFromPlayers(requiredKeys);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RemoveKeysFromPlayers(string[] keysToRemove)
    {
        foreach (PlayerKeyInventory inv in inventoriesInTrigger)
        {
            inv.RemoveKeys(keysToRemove);
        }
    }
}
