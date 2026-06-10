using CombatSystem;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls match-critical state transitions (player death, revive and team wipe restart).
/// </summary>
public class MatchStateManager : NetworkBehaviour, IPlayerLeft
{
[Header("Revive")]
    [Tooltip("Network prefab spawned at the dead player's position and used as the revive point.")]
    [SerializeField] NetworkObject reviveTombstonePrefab;
    [Tooltip("Delay before restarting the stage after everyone is dead.")]
    [SerializeField] float teamWipeRestartDelaySeconds = 2f;

    /// <summary>
    /// Keeps one revive tombstone per dead player.
    /// </summary>
    readonly Dictionary<PlayerRef, NetworkObject> tombstonesByPlayer = new();

    /// <summary>
    /// Scene snapshot captured when this match starts; used for restart.
    /// </summary>
    SceneRef currentMatchScene;

    TickTimer restartTimer;
    bool restartQueued;

    /// <summary>
    /// Caches the current match scene so team wipe restart always reloads the same scene.
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();
        currentMatchScene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Authoritative entry point called when a player dies.
    /// Spawns a tombstone and checks if a team wipe happened.
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void NotifyPlayerDeathRpc(PlayerRef deadPlayer, Vector3 deathPosition)
    {
        if (deadPlayer == PlayerRef.None)
            return;

        SpawnTombstoneIfNeeded(deadPlayer, deathPosition);
        EvaluateMatchState();
    }

    /// <summary>
    /// Convenience wrapper to notify a death using the player's network identity.
    /// </summary>
    public void NotifyPlayerDeath(PlayerHealth playerHealth, Vector3 deathPosition)
    {
        if (playerHealth == null)
            return;

        PlayerRef deadPlayer = playerHealth.GetPlayerRef();
        if (deadPlayer == PlayerRef.None)
            return;

        NotifyPlayerDeathRpc(deadPlayer, deathPosition);
    }

    /// <summary>
    /// Returns true when a player has a spawned PlayerHealth and is not dead.
    /// </summary>
    public bool IsPlayerAlive(PlayerRef player)
    {
        if (player == PlayerRef.None)
            return false;

        if (TryGetPlayerHealth(player, out var health))
            return !health.IsDead();

        return false;
    }

    /// <summary>
    /// Finalizes revive from a tombstone and removes it from the world.
    /// </summary>
    public void TryRevivePlayerFromTombstone(ReviveTombstone tombstone)
    {
        if (!HasStateAuthority || tombstone == null)
            return;

        PlayerRef deadPlayer = tombstone.DeadPlayer;
        if (deadPlayer == PlayerRef.None)
            return;

        if (TryGetPlayerHealth(deadPlayer, out var deadPlayerHealth) && deadPlayerHealth.IsDead())
        {
            deadPlayerHealth.RPC_Revive();
        }

        RemoveTombstone(deadPlayer);
        EvaluateMatchState();
    }

    /// <summary>
    /// Spawns the revive tombstone for a dead player if there is no valid one yet.
    /// </summary>
    void SpawnTombstoneIfNeeded(PlayerRef deadPlayer, Vector3 deathPosition)
    {
        if (!HasStateAuthority)
            return;

        if (reviveTombstonePrefab == null)
            return;

        if (tombstonesByPlayer.TryGetValue(deadPlayer, out var existingTombstone))
        {
            if (existingTombstone != null && existingTombstone.IsValid)
                return;

            tombstonesByPlayer.Remove(deadPlayer);
        }

        NetworkObject tombstoneObject = Runner.Spawn(
            reviveTombstonePrefab,
            deathPosition,
            Quaternion.identity
        );

        ReviveTombstone tombstone = tombstoneObject.GetComponent<ReviveTombstone>();
        if (tombstone != null)
            tombstone.Initialize(deadPlayer, deathPosition);

        tombstonesByPlayer[deadPlayer] = tombstoneObject;
    }

    /// <summary>
    /// Evaluates the number of alive players and schedules/cancels restart accordingly.
    /// </summary>
    void EvaluateMatchState()
    {
        if (!HasStateAuthority)
            return;

        int aliveCount = 0;
        var playerHealths = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        foreach (var health in playerHealths)
        {
            PlayerRef playerRef = health.GetPlayerRef();
            if (playerRef == PlayerRef.None)
                continue;

            if (!health.IsDead())
                aliveCount++;
        }

        if (aliveCount == 0)
        {
            restartQueued = true;
            restartTimer = TickTimer.CreateFromSeconds(Runner, teamWipeRestartDelaySeconds);
        }
        else
        {
            restartQueued = false;
            restartTimer = TickTimer.None;
        }
    }

    /// <summary>
    /// Processes delayed actions tied to simulation ticks (e.g. team wipe restart).
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        if (restartQueued && restartTimer.Expired(Runner))
        {
            restartQueued = false;
            restartTimer = TickTimer.None;
            Runner.LoadScene(currentMatchScene);
        }
    }

    /// <summary>
    /// Resolves the spawned player object and returns its PlayerHealth component.
    /// </summary>
    bool TryGetPlayerHealth(PlayerRef playerRef, out PlayerHealth playerHealth)
    {
        playerHealth = null;
        var players = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        foreach (var health in players)
        {
            if (health.GetPlayerRef() != playerRef)
                continue;

            playerHealth = health;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Despawns and removes the tombstone mapped to a player.
    /// </summary>
    void RemoveTombstone(PlayerRef deadPlayer)
    {
        //if (!tombstonesByPlayer.TryGetValue(deadPlayer, out var tombstoneObject))
        //    return;

        //if (tombstoneObject != null && tombstoneObject.IsValid)
        //    Runner.Despawn(tombstoneObject);

        //tombstonesByPlayer.Remove(deadPlayer);

        if (!tombstonesByPlayer.TryGetValue(deadPlayer, out var tombstoneObject))
            return;

        StartCoroutine(RemoveTombstoneRoutine(deadPlayer, tombstoneObject));

    }
    IEnumerator RemoveTombstoneRoutine(
    PlayerRef deadPlayer,
    NetworkObject tombstoneObject)
    {
        TombstoneFeedback feedback =
            tombstoneObject.GetComponent<TombstoneFeedback>();

        if (feedback != null)
        {
            feedback.DestroyTombstone();

            yield return new WaitForSeconds(0.5f);
        }

        if (tombstoneObject != null && tombstoneObject.IsValid)
        {
            Runner.Despawn(tombstoneObject);
        }

        tombstonesByPlayer.Remove(deadPlayer);
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (!HasStateAuthority)
            return;

        if (TryGetPlayerKeyInventory(player, out var inventory))
        {
            var keys = inventory.GetKeys();
            if (keys.Count > 0 && Runner.ActivePlayers.Count() > 0)
            {
                inventory.SpawnKeyPickup(keys);
            }
        }
    }

    bool TryGetPlayerKeyInventory(PlayerRef playerRef, out PlayerKeyInventory inventory)
    {
        inventory = null;
        var inventories = FindObjectsByType<PlayerKeyInventory>(FindObjectsSortMode.None);
        foreach (var inv in inventories)
        {
            if (inv.Object.InputAuthority == playerRef)
            {
                inventory = inv;
                return true;
            }
        }

        return false;
    }
}
