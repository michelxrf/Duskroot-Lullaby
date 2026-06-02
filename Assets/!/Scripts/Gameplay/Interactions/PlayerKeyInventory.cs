using Fusion;
using UnityEngine;
using System.Collections.Generic;
using CombatSystem;
using System;

/// <summary>
/// Manages the keys collected by the player.
/// </summary>
public class PlayerKeyInventory : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] NetworkObject pickableKeyPrefab;
    [SerializeField] List<string> collectedKeys = new List<string>();

    PlayerHealth playerHealth;

    Action<List<string>> onKeysChanged;

    public override void Spawned()
    {
        base.Spawned();
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnDied += HandleDied;
        }

        FindFirstObjectByType<UiKeys>().FillKeys(collectedKeys);
        onKeysChanged += FindFirstObjectByType<UiKeys>().FillKeys;
    }

    public override void Despawned(NetworkRunner runner, bool hasGracefulExit)
    {
        base.Despawned(runner, hasGracefulExit);
        if (playerHealth != null)
        {
            playerHealth.OnDied -= HandleDied;
        }
    }

    /// <summary>
    /// Adds a list of keys to the inventory.
    /// </summary>
    public void AddKeys(IEnumerable<string> keys)
    {
        collectedKeys.AddRange(keys);
        onKeysChanged?.Invoke(collectedKeys);
    }

    public void RemoveKeys(IEnumerable<string> keys)
    {
        foreach (string key in keys)
        {
            collectedKeys.Remove(key);
        }
        onKeysChanged?.Invoke(collectedKeys);
    }

    /// <summary>
    /// Returns the list of collected keys.
    /// </summary>
    public List<string> GetKeys()
    {
        return collectedKeys;
    }

    /// <summary>
    /// Clears the inventory.
    /// </summary>
    public void ClearKeys()
    {
        collectedKeys.Clear();
        onKeysChanged?.Invoke(collectedKeys);
    }

    void HandleDied()
    {
        // Only the player themselves handles the drop on death
        if (!HasStateAuthority) return;

        if (collectedKeys.Count > 0)
        {
            SpawnKeyPickup(collectedKeys);
            ClearKeys();
        }
    }

    /// <summary>
    /// Spawns a PickableKey object in the world with the provided keys.
    /// </summary>
    public void SpawnKeyPickup(List<string> keys)
    {
        if (pickableKeyPrefab == null)
        {
            Debug.LogWarning("PickableKey prefab not assigned to PlayerKeyInventory.");
            return;
        }

        NetworkObject keyObj = Runner.Spawn(pickableKeyPrefab, transform.position, Quaternion.identity);
        PickableKey pickableKey = keyObj.GetComponent<PickableKey>();
        if (pickableKey != null)
        {
            pickableKey.Initialize(keys);
        }
    }
}
