using UnityEngine;
using UnityEngine.TextCore.Text;

namespace ProgressionSystem
{
    /// <summary>
    /// Functions for calculating experience requirements and stat upgrades based on character and enemy level and templates.
    /// </summary>
    public static class ExperienceCalculator
    {
        const string pathToEntityTemplates = "Data";

        /// <summary>
        /// Returns the adjusted CharacterData for a character based on their current level
        /// </summary>
        /// <param name="characterData"></param>
        /// <returns></returns>
        public static CharacterData LevelUpCharacter(CharacterData characterData, int newLevel)
        {
            // Load the entity template for this character
            CharacterTemplate characterTemplate = Resources.Load<CharacterTemplate>($"{pathToEntityTemplates}/Characters/{characterData.characterId}");

            if (characterTemplate == null)
            {
                Debug.LogError($"Failed to load EntityTemplate for character: {characterData.characterId}");
                return null;
            }

            // Create a new CharacterData based on the template
            CharacterData upgradedCharacterData = new CharacterData
            {
                characterId = characterData.characterId,
                experience = 0,
                weapon = characterData.weapon,

                experienceToNextLevel = Mathf.FloorToInt(100 * Mathf.Pow(2, (characterData.level))),
                health = characterTemplate.health + (characterTemplate.healthUpgradePerLevel * (characterData.level)),
                armor = characterTemplate.armor + (characterTemplate.armorUpgradePerLevel * (characterData.level)),
                damage = characterTemplate.damage + (characterTemplate.damageUpgradePerLevel * (characterData.level)),
                speed = characterTemplate.speed + (characterTemplate.speedUpgradePerLevel * (characterData.level)),
                attackSpeed = characterTemplate.attackSpeed + (characterTemplate.attackSpeedUpgradePerLevel * (characterData.level)),
                cure = characterTemplate.cure + (characterTemplate.cureUpgradePerLevel * (characterData.level)),
                
                level = newLevel
            };

            return upgradedCharacterData;
        }

        public static EnemyData LevelUpEnemy(EnemyTemplate enemyTemplate, int level)
        {
            EnemyData enemyData = new EnemyData
            {
                CharacterId = enemyTemplate.CharacterId,
                Level = level,

                health = enemyTemplate.health + (level - 1) * enemyTemplate.healthUpgradePerLevel,
                armor = enemyTemplate.armor + (level - 1) * enemyTemplate.armorUpgradePerLevel,
                damage = enemyTemplate.damage + (level - 1) * enemyTemplate.damageUpgradePerLevel,
                speed = enemyTemplate.speed + (level - 1) * enemyTemplate.speedUpgradePerLevel,
                attackSpeed = enemyTemplate.attackSpeed + (level - 1) * enemyTemplate.attackSpeedUpgradePerLevel,
                cure = enemyTemplate.cure + (level - 1) * enemyTemplate.cureUpgradePerLevel,
                weapon = enemyTemplate.weapon,

                experienceReward = enemyTemplate.experienceReward + (level - 1) * enemyTemplate.experienceRewardUpgradePerLevel
            };

            return enemyData;
        }
    }
}
