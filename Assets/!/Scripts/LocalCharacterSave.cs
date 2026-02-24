using UnityEngine;

public static class LocalCharacterSave
{
    private const string KEY = "PLAYER_CHARACTERS";

    public static void Save(PlayerCharactersData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    public static PlayerCharactersData Load()
    {
        if (!PlayerPrefs.HasKey(KEY))
            return null;

        string json = PlayerPrefs.GetString(KEY);
        return JsonUtility.FromJson<PlayerCharactersData>(json);
    }
}