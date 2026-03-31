using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data structure to handle a single character data
/// </summary>
[Serializable]
public class CharacterData
{
    public string characterId;
    public int level;
    public int experience;
    public int experienceToNextLevel;

    // attributes
    public int health;
    public float armor;
    public int damage;
    public int speed;
    public int attackSpeed;
    public int cure;

    public WeaponData weapon;

    public CharacterData(CharacterDataDTO dataDTO)
    {
        characterId = dataDTO.characterId;
        level = dataDTO.level;
        experience = dataDTO.experience;
        health = dataDTO.health;
        armor = dataDTO.armor;
        damage = dataDTO.damage;
        speed = dataDTO.speed;
        attackSpeed = dataDTO.attackSpeed;
        cure = dataDTO.cure;
        experienceToNextLevel = dataDTO.experienceToNextLevel;
        weapon = Resources.Load<WeaponData>($"Data/Weapons/{dataDTO.weapon}");
    }

    public CharacterData() { }
}

[Serializable]
public class CharacterDataDTO
{
    public string characterId;
    public int level;
    public int experience;
    public int experienceToNextLevel;

    // attributes
    public int health;
    public float armor;
    public int damage;
    public int speed;
    public int attackSpeed;
    public int cure;
    public int hurt;

    public string weapon;

    public CharacterDataDTO(CharacterData characterData)
    {
        characterId = characterData.characterId;
        level = characterData.level;
        experience = characterData.experience;
        health = characterData.health;
        armor = characterData.armor;
        damage = characterData.damage;
        speed = characterData.speed;
        attackSpeed = characterData.attackSpeed;
        cure = characterData.cure;
        weapon = characterData.weapon.name;
        experienceToNextLevel = characterData.experienceToNextLevel;
    }
}


/// <summary>
/// Data structure to handle all characters of a single user
/// </summary>
[Serializable]
public class PlayerCharactersData
{
    public CharacterData[] Characters = new CharacterData[4];

    public PlayerCharactersData(CharacterTemplate[] defaultCharacterTemplates)
    {
        for (int i = 0; i < 4; i++)
        {
            Characters[i] = new CharacterData
            {
                characterId = defaultCharacterTemplates[i].CharacterId,
                level = defaultCharacterTemplates[i].Level,
                experience = defaultCharacterTemplates[i].Experience,
                health = defaultCharacterTemplates[i].health,
                armor = defaultCharacterTemplates[i].armor,
                damage = defaultCharacterTemplates[i].damage,
                speed = defaultCharacterTemplates[i].speed,
                attackSpeed = defaultCharacterTemplates[i].attackSpeed,
                cure = defaultCharacterTemplates[i].cure,
                weapon = defaultCharacterTemplates[i].weapon,
                experienceToNextLevel = defaultCharacterTemplates[i].ExperienceToNextLevel
                // TODO: initialize startingItens based from the defaultCharacterTemplates
            };
        }

        Debug.Log("Created default PlayerCharactersData with 4 uninitialized characters.");
    }

    public PlayerCharactersData() { }
    public PlayerCharactersData(PlayerCharactersDataDTO dataDTO)
    {
        foreach (var characterDataDTO in dataDTO.Characters)
        {
            Characters = new CharacterData[dataDTO.Characters.Length];
            for (int i = 0; i < dataDTO.Characters.Length; i++)
            {
                Characters[i] = new CharacterData(dataDTO.Characters[i]);
            }
        }
    }
}



public class PlayerCharactersDataDTO
{
    public CharacterDataDTO[] Characters;
    public PlayerCharactersDataDTO(PlayerCharactersData playerCharactersData)
    {
        foreach (var characterData in playerCharactersData.Characters)
        {
            Characters = new CharacterDataDTO[playerCharactersData.Characters.Length];
            for (int i = 0; i < playerCharactersData.Characters.Length; i++)
            {
                Characters[i] = new CharacterDataDTO(playerCharactersData.Characters[i]);
            }
        }
    }
}
