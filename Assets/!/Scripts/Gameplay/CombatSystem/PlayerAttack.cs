using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        Walk,
        DebugRevive
    }

    /// <summary>
    /// Handles player attacking behavior including weapon equipping and attack execution.
    /// Manages weapon switching and coordinates with weapon behavior systems.
    /// </summary>
    public class PlayerAttack : NetworkBehaviour
    {
        [SerializeField] Transform hitboxCenter;
        [SerializeField] InputButton assignedButton;
        [SerializeField] WeaponData defaultWeapon;

        [SerializeField] GameObject[] weaponModels;

        WeaponDataInstance currentWeapon;
        WeaponBehavior weaponBehavior;
        CharacterLook characterLook;
        Animator animator;

        bool lastAttack = false; // to make it so attack only triggers on button down

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            if (GetInput(out NetworkInputData data))
            {
                bool isButtonPressed = GetButtonInput(data, assignedButton);
                if (isButtonPressed && !lastAttack)
                {
                    weaponBehavior.Execute();
                    RPC_PlayAttackAnim();
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

            CharacterData playerCharacter = CharacterDataManager.Instance.GetCurrentPlayerCharacter();
            EquipWeapon(playerCharacter.weapon, playerCharacter.weaponLevel, playerCharacter.weaponSeed);
        }

        /// <summary>
        /// Equips a weapon to the player character.
        /// Handles instantiating the weapon model, setting up animations, and initializing weapon behavior.
        /// </summary>
        /// <param name="newWeapon">The weapon data to equip</param>
        public void EquipWeapon(WeaponData newWeapon, int weaponLevel, string weaponSeed)
        {
            if (newWeapon == null)
            {
                Debug.LogWarning("No weapon assigned, equipping default weapon.");
                EquipWeapon(defaultWeapon, 0, "1");
                return;
            }

            Debug.Log("Equipping weapon: " + newWeapon.name + " at level " + weaponLevel);

            currentWeapon = new WeaponDataInstance(newWeapon, weaponLevel, weaponSeed);

            if (HasStateAuthority)
            {
                if(weaponLevel < 0)
                    Log.Error("Weapon level cannot be negative");

                // disbable existing weapon behavior if any before initializing the new one
                WeaponBehavior[] existingBehaviors = GetComponents<WeaponBehavior>();
                foreach (var behavior in existingBehaviors)
                {
                    Destroy(behavior);
                }

                // save player data
                CharacterDataManager.Instance.SaveWeaponData(CharacterDataManager.Instance.GetCurrentPlayerCharacter(), currentWeapon);

                // Initialize weapon behavior on state authority
                weaponBehavior = WeaponBehaviorFactory.CreateBehavior(currentWeapon.weaponData.behavior[weaponLevel], gameObject);
                weaponBehavior.Initialize(hitboxCenter, animator, gameObject, currentWeapon);

                RPC_EquipWeaponSync(currentWeapon.weaponData.name, weaponLevel);
                RPC_PlayEquipAnimation();
            }

            FindFirstObjectByType<WeaponCard>().UpdateWeapon(currentWeapon.weaponData, weaponLevel);
            GetComponent<PlayerSetup>().SetCurrentWeapon(currentWeapon.weaponData.name);
            
        }

        /// <summary>
        /// RPC to play the weapon equip animation across all network clients.
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void RPC_PlayEquipAnimation()
        {
            animator?.SetTrigger("Equip");
        }

        /// <summary>
        /// RPC to synchronize weapon equipping across all network clients.
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void RPC_EquipWeaponSync(string weaponName, int weaponLevel)
        {
            WeaponDataInstance syncedWeapon = currentWeapon;

            // Non-authority clients need to load the weapon data by name
            if (!HasStateAuthority)
            {
                WeaponData weaponSO = Resources.Load<WeaponData>($"Data/Weapons/Player/{weaponName}");
                syncedWeapon = new WeaponDataInstance(weaponSO, weaponLevel, System.Guid.NewGuid().ToString());
                if (syncedWeapon == null)
                {
                    return;
                }
                currentWeapon = syncedWeapon;
            }

            animator.runtimeAnimatorController = syncedWeapon.weaponData.animationController;

            foreach (var model in weaponModels)
            {
                model.SetActive(model.name == syncedWeapon.weaponData.weaponModelName[syncedWeapon.weaponLevel]);
            }
        }

        /// <summary>
        /// RPC to execute the current weapon's attack across all network clients.
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void RPC_ExecuteAttack()
        {
            weaponBehavior.Execute();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_PlayAttackAnim()
        {
            animator?.SetTrigger("Attack");
        }

        public string GetCurrentWeaponName()
        {
            return currentWeapon != null ? currentWeapon.weaponData.name : "No Weapon";
        }

        public WeaponDataInstance GetCurrentWeaponData()
        {
            if(currentWeapon.weaponData.name == "Unarmed")
            {
                return null;
            }
            return currentWeapon;
        }
    }
}

