using UnityEngine;
using System.Collections;
using Fusion;

/// <summary>
/// Abstract base class for weapon behavior implementations.
/// Defines the interface and common functionality for all weapon types.
/// </summary>
public class WeaponBehavior : NetworkBehaviour
{
    protected Animator animator;
    protected GameObject owner;
    protected Transform defaultTarget;
    protected WeaponData weaponData;
    protected bool isOnCooldown = false;

    public override void Spawned()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Executes the weapon's attack action if not on cooldown.
    /// Automatically handles cooldown management based on attack speed.
    /// </summary>
    public virtual void Execute()
    {
        if (isOnCooldown) return;

        StartCoroutine(StartCooldown(1/(CharacterDataManager.Instance.GetCurrentPlayerCharacter().attackSpeed)));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayAttackAnim()
    {
        animator?.SetTrigger("Attack");
    }

    /// <summary>
    /// Called at the impact frame of an attack animation.
    /// Override this method to apply effects (damage, knockback, etc.) at the right time.
    /// </summary>
    public virtual void ImpactFrame()
    {
        if(!HasStateAuthority) return;
    }

    /// <summary>
    /// Initializes the weapon behavior with required components and data.
    /// </summary>
    /// <param name="defaultTarget">The default target position (e.g., character's hitbox)</param>
    /// <param name="anim">The animator component for playing attack animations</param>
    /// <param name="owner">The game object that owns this weapon</param>
    /// <param name="weaponData">The data defining this weapon's properties</param>
    public virtual void Initialize(Transform defaultTarget, Animator anim, GameObject owner, WeaponData weaponData)
    {
        animator = anim;
        this.weaponData = weaponData;
        this.owner = owner;
        this.defaultTarget = defaultTarget;
    }

    /// <summary>
    /// Handles the cooldown timer for attack rate limiting.
    /// </summary>
    /// <param name="time">The cooldown duration in seconds</param>
    /// <returns>Coroutine enumerator</returns>
    IEnumerator StartCooldown(float time)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(time);
        isOnCooldown = false;
    }

    /// <summary>
    /// Stops all coroutines when the weapon behavior is destroyed to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
