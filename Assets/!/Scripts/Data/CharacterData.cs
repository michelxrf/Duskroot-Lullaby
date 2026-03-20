using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data structure to handle a single character data
/// </summary>
[Serializable]
public class CharacterData
{
    public string CharacterId;
    public int Level;
    public int Experience;

    // attributes
    public int health;
    public float armor;
    public int damage;
    public int speed;
    public int attackSpeed;
    public int cure;
    public int hurt;

    public WeaponData weapon;

    public CharacterData(CharacterDataDTO dataDTO)
    {
        CharacterId = dataDTO.CharacterId;
        Level = dataDTO.Level;
        Experience = dataDTO.Experience;
        health = dataDTO.health;
        armor = dataDTO.armor;
        damage = dataDTO.damage;
        speed = dataDTO.speed;
        attackSpeed = dataDTO.attackSpeed;
        cure = dataDTO.cure;
        hurt = dataDTO.hurt;

        weapon = Resources.Load<WeaponData>($"Assets/!/Data/Weapons/{dataDTO.weapon}");
    }

    public CharacterData() { }
}

[Serializable]
public class CharacterDataDTO
{
    public string CharacterId;
    public int Level;
    public int Experience;

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
        CharacterId = characterData.CharacterId;
        Level = characterData.Level;
        Experience = characterData.Experience;
        health = characterData.health;
        armor = characterData.armor;
        damage = characterData.damage;
        speed = characterData.speed;
        attackSpeed = characterData.attackSpeed;
        cure = characterData.cure;
        hurt = characterData.hurt;
        weapon = characterData.weapon.name;
    }
}


/// <summary>
/// Data structure to handle all characters of a single user
/// TODO: add an initialization that create a set of actual character for the new user, instead of just uninitialized characters
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
                CharacterId = defaultCharacterTemplates[i].CharacterId,
                Level = defaultCharacterTemplates[i].Level,
                Experience = defaultCharacterTemplates[i].Experience,
                health = defaultCharacterTemplates[i].health,
                armor = defaultCharacterTemplates[i].armor,
                damage = defaultCharacterTemplates[i].damage,
                speed = defaultCharacterTemplates[i].speed,
                attackSpeed = defaultCharacterTemplates[i].attackSpeed,
                cure = defaultCharacterTemplates[i].cure,
                hurt = defaultCharacterTemplates[i].hurt,
                weapon = defaultCharacterTemplates[i].weapon,

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
