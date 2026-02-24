using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data structure to handle a single character data
/// </summary>
[Serializable]
public class CharacterData
{
    public int Level;
    public int Experience;
    public string CharacterId;
    public Dictionary<string, int> Inventory = new Dictionary<string, int>();
}

/// <summary>
/// Data structure to handle all characters of a single user
/// TODO: add an initialization that create a set of actual character for the new user, instead of just uninitialized characters
/// </summary>
[Serializable]
public class PlayerCharactersData
{
    public CharacterData[] Characters = new CharacterData[4];

    public static PlayerCharactersData CreateDefault()
    {
        var data = new PlayerCharactersData();

        for (int i = 0; i < 4; i++)
        {
            data.Characters[i] = new CharacterData
            {
                Level = 1,
                Experience = 0,
                CharacterId = "Uninitialized",
                Inventory = new Dictionary<string, int>()
            };
        }

        Debug.Log("Created default PlayerCharactersData with 4 uninitialized characters.");
        return data;
    }
}

