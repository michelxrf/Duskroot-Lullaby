using CombatSystem;
using Fusion;
using UnityEngine;

public class TrainingDummy : NetworkBehaviour
{
    Animator animator;

    public override void Spawned()
    {
        animator = GetComponentInChildren<Animator>();
        GetComponent<PropHealth>().OnHealthChanged += Hit;
    }

    private void Hit(int health)
    {
        RPC_PlayHitAnimation();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayHitAnimation()
    {
        animator.SetTrigger("Hit");
    }
}
