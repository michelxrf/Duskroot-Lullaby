using UnityEngine;

/// <summary>
/// Utility class for saving and loading player character data to local device storage.
/// Uses PlayerPrefs for persistent local storage of character information.
/// </summary>
public static class LocalCharacterSave
{
    private const string KEY = "PLAYER_CHARACTERS";

    /// <summary>
    /// Saves player character data to local device storage.
    /// </summary>
    /// <param name="data">The player characters data to save</param>
    public static void Save(PlayerCharactersData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads player character data from local device storage.
    /// </summary>
    /// <returns>The loaded player characters data, or null if no data exists</returns>
    public static PlayerCharactersData Load()
    {
        if (!PlayerPrefs.HasKey(KEY))
            return null;

        string json = PlayerPrefs.GetString(KEY);
        return JsonUtility.FromJson<PlayerCharactersData>(json);
    }
}