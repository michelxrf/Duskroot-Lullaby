using UnityEngine;

namespace ProgressionSystem
{
    /// <summary>
    /// Functions for calculating experience requirements and stat upgrades based on character and enemy level and templates.
    /// </summary>
    public static class ExperienceCalculator
    {
        const string pathToEntityTemplates = "Data/";

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
                level = newLevel,
                experience = characterData.experience,
                weapon = characterData.weapon,

                experienceToNextLevel = Mathf.FloorToInt(characterTemplate.ExperienceToNextLevel + characterTemplate.experienceRequirementMultiplier * (characterData.level - 1)),
                health = characterTemplate.health + (characterTemplate.healthUpgradePerLevel * (characterData.level - 1)),
                armor = characterTemplate.armor + (characterTemplate.armorUpgradePerLevel * (characterData.level - 1)),
                damage = characterTemplate.damage + (characterTemplate.damageUpgradePerLevel * (characterData.level - 1)),
                speed = characterTemplate.speed + (characterTemplate.speedUpgradePerLevel * (characterData.level - 1)),
                attackSpeed = characterTemplate.attackSpeed + (characterTemplate.attackSpeedUpgradePerLevel * (characterData.level - 1)),
                cure = characterTemplate.cure + (characterTemplate.cureUpgradePerLevel * (characterData.level - 1))
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
