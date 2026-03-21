using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CombatSystem
{
    /// <summary>
    /// Handles player attacking behavior
    /// </summary>
    public class PlayerAttack : NetworkBehaviour
    {
        [SerializeField] Transform hitboxCenter;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        
        public CharacterData characterData;
        public WeaponData currentWeapon;
        public WeaponBehavior weaponBehavior;
        CharacterLook characterLook;
        Animator animator;
        GameObject equippedWeaponModel;
        bool lastAttack = false; // to make it so attack only triggers on button down

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            if (GetInput(out NetworkInputData data))
            {
                if (data.Attack && !lastAttack)
                {
                    RPC_ExecuteAttack();
                }

                lastAttack = data.Attack;
            }
        }

        public override void Spawned()
        {
            animator = GetComponentInChildren<Animator>();
            characterLook = GetComponent<CharacterLook>();
            EquipWeapon(CharacterDataManager.Instance.GetCurrentPlayerCharacter().weapon);
        }

        void EquipWeapon(WeaponData newWeapon)
        {
            if (newWeapon == null) return;

            currentWeapon = newWeapon;
            if (equippedWeaponModel != null)
                Destroy(equippedWeaponModel);

            if (currentWeapon.weaponModel != null)
            {
                Transform hand = newWeapon.rigthHanded ? rightHand : leftHand;
                GameObject model = Instantiate(currentWeapon.weaponModel, hand);
            }

            animator.runtimeAnimatorController = currentWeapon.animationController;

            weaponBehavior = WeaponBehaviorFactory.CreateBehavior(currentWeapon.behavior, gameObject);
            weaponBehavior.Initialize(hitboxCenter, animator, gameObject, currentWeapon);
            RPC_PlayEquipAnimation();
            //TODO: play SFX
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        void RPC_PlayEquipAnimation()
        {
            //animator.SetTrigger("Equip");
            // TODO: Add equip animation and trigger to animator
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        void RPC_ExecuteAttack()
        {
            weaponBehavior.Execute();
        }

        public void ImpactFrame()
        {
            weaponBehavior.ImpactFrame();
        }
    }
}

