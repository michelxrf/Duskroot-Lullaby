using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

/// <summary>
/// Handles melee aiming and attack behavior for a character.
/// </summary>
public class MeleeAttack : NetworkBehaviour
{
    [SerializeField] int damageAmount = 25;
    [SerializeField] Transform hitboxCenter;
    [SerializeField] float hitboxRadius = 0.5f;

    Animator animator;

    public override void Spawned()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void InitiateAttack()
    {
        if (!HasStateAuthority) return;

        animator.SetTrigger("Attack");
    }

    

    /// <summary>
    /// Callback for animation event to perform hit detection.
    /// </summary>
    public void CastHitBox()
    {
        Collider[] hits = Physics.OverlapSphere(hitboxCenter.position, hitboxRadius);
        
        foreach (var hit in hits)
        {
            if(hit.gameObject == this.gameObject) continue; // Ignore self
            if(hit.enabled == false) continue; // Ignore disabled colliders

            Health healthComponent = hit.GetComponent<Health>();
            if (healthComponent == null) continue;

            healthComponent.RPCTakeDamage(damageAmount);
        }
    }

}
