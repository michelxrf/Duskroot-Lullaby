using Fusion;
using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// Handles attacks including playing attack animations and performing hit detection.
    /// </summary>
    public class Attack : NetworkBehaviour
    {
        [SerializeField] Transform hitboxCenter;
        [SerializeField] WeaponData currentWeapon;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;

        Animator animator;
        GameObject equippedWeaponModel;

        public override void Spawned()
        {
            animator = GetComponentInChildren<Animator>();
            EquipWeapon(currentWeapon);
        }

        void EquipWeapon(WeaponData newWeapon)
        {
            if(newWeapon == null) return;

            currentWeapon = newWeapon;
            if (equippedWeaponModel != null) 
                Destroy(equippedWeaponModel);
            
            if (currentWeapon.weaponModel != null)
            {
                Transform hand = newWeapon.rigthHanded ? rightHand : leftHand;
                GameObject model = Instantiate(currentWeapon.weaponModel, hand);
            }

            animator.runtimeAnimatorController = currentWeapon.animationController;
            //TODO: play equip animation
            //TODO: play SFX
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
            CombatFuncs.CastHitBox(hitboxCenter, this.gameObject, currentWeapon);
        }
    }

}
