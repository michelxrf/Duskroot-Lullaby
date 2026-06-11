using CombatSystem;
using Fusion;
using MoreMountains.Feedbacks;
using UnityEngine;

/// <summary>
/// Networked revive interactable created when a player dies.
/// </summary>
public class ReviveTombstone : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int requiredButtonPresses = 8;
    [SerializeField] float interactionRadius = 2f;

    [Networked] public PlayerRef DeadPlayer { get; private set; }
    [Networked] public int ReviveProgress { get; private set; }
    [Networked] public NetworkBool IsCompleted { get; private set; }
    [Networked] Vector3 NetworkSpawnPosition { get; set; }
    public int RequiredButtonPresses => requiredButtonPresses;
    public float InteractionRadius => interactionRadius;

    MatchStateManager matchStateManager;
    SphereCollider interactionTrigger;


    public override void Spawned()
    {
        matchStateManager = FindFirstObjectByType<MatchStateManager>();
        interactionTrigger = GetComponent<SphereCollider>();

        if (interactionTrigger != null)
        {
            interactionTrigger.isTrigger = true;
            interactionTrigger.radius = interactionRadius;
        }

        // Enforce deterministic position across peers even when the prefab has no network transform component.
        if (NetworkSpawnPosition != Vector3.zero)
            transform.position = NetworkSpawnPosition;
        GetComponent<TombstoneFeedback>()?.PlaySpawn();
    }

    public void Initialize(PlayerRef deadPlayer, Vector3 spawnPosition)
    {
        if (!HasStateAuthority)
            return;

        DeadPlayer = deadPlayer;
        ReviveProgress = 0;
        IsCompleted = false;
        NetworkSpawnPosition = spawnPosition;
        transform.position = spawnPosition;
    }

    public void TryInteract()
    {
        if (!Object || !Object.IsValid || IsCompleted)
            return;

        RPC_RequestReviveProgress();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RequestReviveProgress(RpcInfo info = default)
    {
        if (IsCompleted)
            return;

        if (matchStateManager == null)
            return;

        PlayerRef reviver = info.Source;
        if (reviver == PlayerRef.None)
            return;

        if (!matchStateManager.IsPlayerAlive(reviver))
            return;

        if (!IsReviverInsideRange(reviver))
            return;

        ReviveProgress++;

        if (ReviveProgress < requiredButtonPresses)
            return;

        IsCompleted = true;
        GetComponent<TombstoneFeedback>()?.DestroyTombstone();
        matchStateManager.TryRevivePlayerFromTombstone(this);
    }

    bool IsReviverInsideRange(PlayerRef reviver)
    {
        var playerHealths = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        PlayerHealth reviverHealth = null;
        foreach (var health in playerHealths)
        {
            if (health.GetPlayerRef() == reviver)
            {
                reviverHealth = health;
                break;
            }
        }

        if (reviverHealth == null)
            return false;

        float maxDistance = interactionRadius + 0.3f;
        float sqrDistance = (reviverHealth.transform.position - transform.position).sqrMagnitude;
        return sqrDistance <= maxDistance * maxDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerInteractor>()?.EnteredReviveArea(this);
    }

    void OnTriggerExit(Collider other)
    {
        other.GetComponent<PlayerInteractor>()?.LeftReviveArea(this);
    }

    public override void Render()
    {
        base.Render();

        if (NetworkSpawnPosition != Vector3.zero)
            transform.position = NetworkSpawnPosition;
    }
}
