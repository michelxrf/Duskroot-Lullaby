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
        static public void CastHitBox(Transform hitboxCenter, GameObject caster, WeaponDataInstance weapon)
        {
            int layerMask;


            if (caster.layer == LayerMask.NameToLayer("Player"))
                layerMask = LayerMask.GetMask("Monster", "Player", "DestructableProp"); // Player damage both monsters and other players (friendly fire)
            else
                layerMask = LayerMask.GetMask("Player"); // Monsters only damage players

            Collider[] hits = Physics.OverlapSphere(hitboxCenter.position, weapon.hitboxRadius, layerMask);

            foreach (var hit in hits)
            {
                if (hit.gameObject == caster) continue; // Ignore self
                if (hit.enabled == false) continue; // Ignore disabled colliders

                Health healthComponent = hit.GetComponent<Health>();
                if (healthComponent == null) continue;

                int totalDamage;

                if (caster.GetComponent<PlayerSetup>() != null)
                {
                    totalDamage = weapon.damage + CharacterDataManager.Instance.GetCurrentPlayerCharacter().damage;
                    healthComponent.RPC_TakeDamage(totalDamage, weapon.weaponData.weaponAudioType);
                }
                else if(caster.GetComponent<EnemySetup>() != null)
                {
                    totalDamage = caster.GetComponent<EnemySetup>().GetEnemyData().damage + weapon.damage;
                    healthComponent.RPC_TakeDamage(totalDamage, weapon.weaponData.weaponAudioType);
                }

                // apply knockback if the hit object has a Knockback component
                if(!healthComponent.IsDead() && !healthComponent.IsInvulnerable)
                {
                    Vector3 direction = new Vector3(hit.transform.position.x - caster.transform.position.x, 0, hit.transform.position.z - caster.transform.position.z).normalized;
                    hit.GetComponent<Knockback>()?.RPC_ApplyKnockback(direction, weapon.knockbackForce);
                }
            }
        }
    }
}


