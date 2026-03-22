using CombatSystem;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

/// <summary>
/// Implementation of melee weapon behavior.
/// Handles the execution of melee attacks including animation triggers and hitbox detection.
/// </summary>
public class Melee : WeaponBehavior
{
    /// <summary>
    /// Executes a melee attack by triggering the attack animation.
    /// </summary>
    public override void Execute()
    {
        base.Execute();
        animator.SetTrigger("Attack");
    }

    /// <summary>
    /// Called at the impact frame of the melee attack animation.
    /// Casts a hitbox to detect and damage enemies at the target position.
    /// </summary>
    public override void ImpactFrame()
    {
        base.ImpactFrame();
        CombatFuncs.CastHitBox(defaultTarget, owner, weaponData);
    }

    /// <summary>
    /// Initializes the melee weapon behavior with required components.
    /// </summary>
    /// <param name="defaultTarget">The target position for the hitbox</param>
    /// <param name="anim">The animator component</param>
    /// <param name="owner">The character wielding this weapon</param>
    /// <param name="weaponData">The weapon configuration data</param>
    public override void Initialize(Transform defaultTarget, Animator anim, GameObject owner, WeaponData weaponData)
    {
        base.Initialize(defaultTarget, anim, owner, weaponData);
    }
}
