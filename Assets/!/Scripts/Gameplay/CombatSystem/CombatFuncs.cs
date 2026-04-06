using UnityEngine;
using ProgressionSystem;

namespace CombatSystem
{
    /// <summary>
    /// Utility class containing static helper functions for combat operations.
    /// </summary>
    public static class CombatFuncs
    {
        /// <summary>
        /// Performs hit detection in a sphere around the hitbox center.
        /// Applies damage to all valid targets hit by the attack.
        /// This is typically called at the impact frame of an attack animation.
        /// </summary>
        /// <param name="hitboxCenter">The center position of the attack hitbox</param>
        /// <param name="caster">The character performing the attack (to ignore self)</param>
        /// <param name="weapon">The weapon data containing damage and hitbox radius information</param>
        static public void CastHitBox(Transform hitboxCenter, GameObject caster, WeaponData weapon)
        {
            int layerMask;


            if (caster.layer == LayerMask.NameToLayer("Player"))
                layerMask = LayerMask.GetMask("Monster", "Player"); // Player damage both monsters and other players (friendly fire)
            else
                layerMask = LayerMask.GetMask("Player"); // Monsters only damage players

            Collider[] hits = Physics.OverlapSphere(hitboxCenter.position, weapon.hitboxRadius, layerMask);

            foreach (var hit in hits)
            {
                if (hit.gameObject == caster) continue; // Ignore self
                if (hit.enabled == false) continue; // Ignore disabled colliders

                Health healthComponent = hit.GetComponent<Health>();
                if (healthComponent == null) continue;

                if (caster.GetComponent<PlayerSetup>() != null)
                {
                    healthComponent.RPC_TakeDamage(weapon.baseDamage + CharacterDataManager.Instance.GetCurrentPlayerCharacter().damage, weapon.weaponAudioType);
                }
                else
                {
                    healthComponent.RPC_TakeDamage(caster.GetComponent<EnemySetup>().GetEnemyData().damage, weapon.weaponAudioType);
                }
            }
        }
    }
}


