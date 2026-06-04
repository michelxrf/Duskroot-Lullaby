using CombatSystem;
using Fusion;
using UnityEngine;

/// <summary>
/// Automatically emits a floating damage bubble above the character whenever the health changes (e.g., when taking damage).
/// </summary>
[RequireComponent(typeof(Health))]
public class FloatingDamageEmmiter : NetworkBehaviour
{
    Health healthComponent;
    [SerializeField] GameObject damageBubblePrefab;

    public override void Spawned()
    {
        healthComponent = GetComponent<Health>();
        healthComponent.OnReceivedDamage += Emit;
    }

    void Emit(int damageAmount)
    {
        RPC_Emit(damageAmount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_Emit(int damageAmount)
    {
        var newBubble = Runner.Spawn(damageBubblePrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        newBubble.GetComponent<FloatingDamageBubble>().Init(damageAmount);
    }
}
