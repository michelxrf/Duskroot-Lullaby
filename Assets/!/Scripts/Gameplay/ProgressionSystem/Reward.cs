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
        [SerializeField] GameObject[] dropItens;
        [Range(0f, 1f)]
        [SerializeField] float[] dropRates;
        [SerializeField] float chanceToDropItem = 0.1f;

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
            if (dropItens.Length == 0 || dropRates.Length == 0 || dropItens.Length != dropRates.Length)
                return;
            
            float randomValue = UnityEngine.Random.value;
            if (randomValue > chanceToDropItem)
                return;
            foreach (var item in dropItens)
            {
                float dropChance = dropRates[Array.IndexOf(dropItens, item)];
                if (UnityEngine.Random.value <= dropChance)
                {
                    DropItem(item);
                }
            }
        }

        void DropItem(GameObject item)
        {
            // Instantiate the item in the game world at the reward's position
            Instantiate(item, transform.position, Quaternion.identity);
        }
    }
}
