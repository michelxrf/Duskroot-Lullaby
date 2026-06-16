using CombatSystem;
using Fusion;
using System;
using UnityEngine;
using static AudioPunchPlayer;

namespace ProgressionSystem
{
    /// <summary>
    /// Represents a reward object that grants experience points to the player when collected or triggered.
    /// </summary>
    public class Reward : NetworkBehaviour
    {
        [Networked] public int experienceAmount { get; set; }

        [Header("Healing Items")]
        [Range(0f, 1f)]
        [SerializeField] float healingDropChance = 0.2f;
        [Tooltip("List of healing items to drop.")]
        [SerializeField] GameObject[] healingItemPrefabs;

        [Header("Weapons")]
        [Range(0f, 1f)]
        [SerializeField] float weaponDropChance = 0.3f;
        [Tooltip("Weapon Scriptable Objects to drop")]
        [SerializeField] GameObject[] weapons;

        /// <summary>
        /// Applies the reward by adding experience to the player's current character.
        /// </summary>
        public void ApplyReward()
        {
            CharacterDataManager.Instance.AddExperience(experienceAmount);
            if (HasStateAuthority)
            {
                DropRandomItem();
            }
        }

        public override void Spawned()
        {
            if (HasStateAuthority)
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
        }

        void DropRandomItem()
        {
            if (weapons.Length == 0 && healingItemPrefabs.Length == 0)
                return; // Nothing to drop

            float randomValue = UnityEngine.Random.Range(0f, 1f);
            float cumulativeChance = 0f;

            // Try weapon drop first
            if (weapons.Length > 0)
            {
                cumulativeChance += weaponDropChance;
                if (randomValue < cumulativeChance)
                {
                    DropWeapon();
                    return;
                }
            }

            // Try healing item drop
            if (healingItemPrefabs.Length > 0)
            {
                cumulativeChance += healingDropChance;
                if (randomValue < cumulativeChance)
                {
                    DropHealing();
                }
            }
        }

        void DropWeapon()
        {
            string weaponSeed = System.Guid.NewGuid().ToString();
            Runner.Spawn(weapons[UnityEngine.Random.Range(0, weapons.Length)], transform.position, Quaternion.identity);
        }

        void DropHealing()
        {
            Debug.Log("Dropping healing item");
            GameObject healingItem = healingItemPrefabs[UnityEngine.Random.Range(0, healingItemPrefabs.Length)];
            Runner.Spawn(healingItem, transform.position, Quaternion.identity);
        }
    }
}
