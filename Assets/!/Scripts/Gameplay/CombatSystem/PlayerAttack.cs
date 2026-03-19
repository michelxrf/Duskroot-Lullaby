using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using TMPro;

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
        
        CharacterData characterData;
        WeaponData currentWeapon;
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
            Debug.Log("Equipping weapon: " + CharacterDataManager.Instance.GetCurrentPlayerCharacter().weapon.name);
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
            currentWeapon.Initialize(hitboxCenter, animator, gameObject);
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
            currentWeapon.Execute();
        }

        public void ImpactFrame()
        {
            currentWeapon.ImpactFrame();
        }
    }
}

