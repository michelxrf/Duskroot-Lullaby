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

    public List<InventoryItem> startingItens;
}

/// <summary>
/// Data structure to handle all characters of a single user
/// TODO: add an initialization that create a set of actual character for the new user, instead of just uninitialized characters
/// </summary>
[Serializable]
public class PlayerCharactersData
{
    public CharacterData[] Characters = new CharacterData[4];

    public static PlayerCharactersData CreateDefault(CharacterTemplate[] defaultCharacterTemplates)
    {
        var data = new PlayerCharactersData();
        
        for (int i = 0; i < 4; i++)
        {
            data.Characters[i] = new CharacterData
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

                // TODO: initialize startingItens based from the defaultCharacterTemplates
            };
        }

        Debug.Log("Created default PlayerCharactersData with 4 uninitialized characters.");
        return data;
    }
}

