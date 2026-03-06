using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;

/// <summary>
/// Keeps track of a character's heath and handles damage and death.
/// </summary>
public class Health : NetworkBehaviour
{
    [SerializeField] int maxHealth = 100;
    
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    int CurrentHealth { get; set; }

    Animator animator;


    public override void Spawned()
    {
        if(HasStateAuthority)
            CurrentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Reduces object's health and triggers death if health reaches zero.
    /// </summary>
    /// <param name="damage"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCTakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    void OnHealthChanged()
    {
        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
            Debug.Log($"Health: {CurrentHealth}/{maxHealth}");
        }
    }

    /// <summary>
    /// Processes the object's death.
    /// </summary>
    void Die()
    {
        Debug.Log("Object has died.");

        // disable character     
        gameObject.DisableAllComponents<Collider>();
        gameObject.DisableComponent<SimpleKCC>();

        // Disbable player controls
        gameObject.DisableComponent<PlayerMelee>();
        gameObject.DisableComponent<PlayerMovement>();

        // Disable AI controls
        // TODO

        animator.SetTrigger("Die");
    }


}
