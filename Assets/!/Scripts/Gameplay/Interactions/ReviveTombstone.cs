using Fusion;
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
    }

    public void Initialize(PlayerRef deadPlayer)
    {
        if (!HasStateAuthority)
            return;

        DeadPlayer = deadPlayer;
        ReviveProgress = 0;
        IsCompleted = false;
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
        matchStateManager.TryRevivePlayerFromTombstone(this);
    }

    bool IsReviverInsideRange(PlayerRef reviver)
    {
        NetworkObject reviverObject = Runner.GetPlayerObject(reviver);
        if (reviverObject == null)
            return false;

        float maxDistance = interactionRadius + 0.3f;
        float sqrDistance = (reviverObject.transform.position - transform.position).sqrMagnitude;
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
}
