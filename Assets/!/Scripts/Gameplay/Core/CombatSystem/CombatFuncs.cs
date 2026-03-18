using UnityEngine;

namespace CombatSystem
{
    public static class CombatFuncs
    {
        /// <summary>
        /// Callback for animation event to perform hit detection.
        /// </summary>
        static public void CastHitBox(Transform hitboxCenter, GameObject caster, WeaponData weapon)
        {
            Collider[] hits = Physics.OverlapSphere(hitboxCenter.position, weapon.hitboxRadius);

            foreach (var hit in hits)
            {
                if (hit.gameObject == caster) continue; // Ignore self
                if (hit.enabled == false) continue; // Ignore disabled colliders

                Health healthComponent = hit.GetComponent<Health>();
                if (healthComponent == null) continue;

                healthComponent.RPCTakeDamage(weapon.baseDamage);
            }
        }
    }
}


