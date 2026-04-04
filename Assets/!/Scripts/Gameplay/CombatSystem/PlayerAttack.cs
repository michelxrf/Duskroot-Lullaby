using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CombatSystem
{
    /// <summary>
    /// Enum representing different input buttons that can trigger attack behaviors.
    /// </summary>
    public enum InputButton
    {
        Attack,
        Skill1,
        Skill2,
        Interact,
        Aim,
        Walk
    }

    /// <summary>
    /// Handles player attacking behavior including weapon equipping and attack execution.
    /// Manages weapon switching and coordinates with weapon behavior systems.
    /// </summary>
    public class PlayerAttack : NetworkBehaviour
    {
        [SerializeField] Transform hitboxCenter;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        [SerializeField] InputButton assignedButton;
        [SerializeField] WeaponData defaultWeapon;

        WeaponData currentWeapon;
        WeaponBehavior weaponBehavior;
        CharacterLook characterLook;
        Animator animator;
        GameObject equippedWeaponModel;
        bool lastAttack = false; // to make it so attack only triggers on button down

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            if (GetInput(out NetworkInputData data))
            {
                bool isButtonPressed = GetButtonInput(data, assignedButton);
                if (isButtonPressed && !lastAttack)
                {
                    RPC_ExecuteAttack();
                }

                lastAttack = isButtonPressed;
            }
        }

        /// <summary>
        /// Retrieves the input state for a specific button from the network input data.
        /// </summary>
        /// <param name="data">The network input data</param>
        /// <param name="buttonType">The button to check</param>
        /// <returns>True if the button is pressed, false otherwise</returns>
        bool GetButtonInput(NetworkInputData data, InputButton buttonType)
        {
            var field = typeof(NetworkInputData).GetField(buttonType.ToString());
            if (field != null && field.FieldType == typeof(bool))
            {
                return (bool)field.GetValue(data);
            }
            return false;
        }

        /// <summary>
        /// Called when the player character is spawned in the network.
        /// Initializes the animator and equips the player's starting weapon.
        /// </summary>
        public override void Spawned()
        {
            animator = GetComponentInChildren<Animator>();
            characterLook = GetComponent<CharacterLook>();

            if(HasInputAuthority)
                EquipWeapon(CharacterDataManager.Instance.GetCurrentPlayerCharacter().weapon);
        }

        /// <summary>
        /// Equips a weapon to the player character.
        /// Handles instantiating the weapon model, setting up animations, and initializing weapon behavior.
        /// </summary>
        /// <param name="newWeapon">The weapon data to equip</param>
        void EquipWeapon(WeaponData newWeapon)
        {
            if (newWeapon == null)
            {
                Debug.LogWarning("No weapon assigned, equipping default weapon.");
                EquipWeapon(defaultWeapon);
                return;
            }

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
        }

        /// <summary>
        /// RPC to play the weapon equip animation across all network clients.
        /// </summary>
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        void RPC_PlayEquipAnimation()
        {
            animator.SetTrigger("Equip");
        }

        /// <summary>
        /// RPC to execute the current weapon's attack across all network clients.
        /// </summary>
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        void RPC_ExecuteAttack()
        {
            weaponBehavior.Execute();
        }
    }
}

