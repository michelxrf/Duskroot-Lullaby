using UnityEngine;
using Fusion;
using System;
using CombatSystem;

namespace ProgressionSystem
{
    /// <summary>
    /// Represents a reward object that grants experience points to the player when collected or triggered.
    /// </summary>
    public class Reward : NetworkBehaviour
    {
        [SerializeField] int experienceAmount = 0;

        [Header("Item Drops")]
        [SerializeField] GameObject pickableWeaponPrefab;

        [Header("Healing Items")]
        [Range(0f, 1f)]
        [SerializeField] float healingDropChance = 0.2f;
        [Tooltip("List of healing items to drop.")]
        [SerializeField] GameObject[] healingItemPrefabs;

        [Header("Weapons")]
        [Range(0f, 1f)]
        [SerializeField] float weaponDropChance = 0.3f;
        [Tooltip("Weapon Scriptable Objects to drop")]
        [SerializeField] WeaponData[] weapons;

        /// <summary>
        /// Applies the reward by adding experience to the player's current character.
        /// </summary>
        public void ApplyReward()
        {
            CharacterDataManager.Instance.AddExperience(experienceAmount);
            DropRandomItem();
        }

        public override void Spawned()
        {
            EnemySetup enemySetup = GetComponent<EnemySetup>();

            if (enemySetup != null)
            {
                if (!enemySetup.IsInitialized())
                {
                    enemySetup.OnInit += () =>
                    {
                        experienceAmount = enemySetup.GetEnemyData().experienceReward;
                    };
                }
                else
                {
                    experienceAmount = enemySetup.GetEnemyData().experienceReward;
                }
            }
                
        }

        void DropRandomItem()
        {
            if (weapons.Length == 0 && healingItemPrefabs.Length == 0)
                return; // Nothing to drop
            
            float randomValue = UnityEngine.Random.value;

            // Try weapon drop first
            if (randomValue < weaponDropChance && weapons.Length > 0)
            {
                DropWeapon();
                return;
            }

            // Try healing item drop
            if (randomValue < weaponDropChance + healingDropChance && healingItemPrefabs.Length > 0)
            {
                DropHealing();
            }
        }

        void DropWeapon()
        {
            WeaponData weaponSO = weapons[UnityEngine.Random.Range(0, weapons.Length)];
            int weaponLevel = GetRandomWeaponLevel();
            string weaponSeed = System.Guid.NewGuid().ToString();
            Runner.Spawn(pickableWeaponPrefab, transform.position, Quaternion.identity).GetComponent<PickableWeapon>().Initialize(weaponSO, weaponLevel, weaponSeed);
        }

        int GetRandomWeaponLevel()
        {
            float randomValue = UnityEngine.Random.value;

            if (randomValue < 0.05f)
                return 3; // Lendária: 5%
            if (randomValue < 0.20f)
                return 2; // Rara: 15%
            if (randomValue < 0.50f)
                return 1; // Incomum: 30%
            return 0; // Comum: 50%
        }

        void DropHealing()
        {
            GameObject healingItem = healingItemPrefabs[UnityEngine.Random.Range(0, healingItemPrefabs.Length)];
            Runner.Spawn(healingItem, transform.position, Quaternion.identity);
        }
    }
}
