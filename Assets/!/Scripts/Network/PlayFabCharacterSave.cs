using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


/// <summary>
/// Function class to handle saving and loading player character data to PlayFab cloud save
/// </summary>
public static class PlayFabCharacterSave
{
    private const string KEY = "PLAYER_CHARACTERS";

    public static void Save(PlayerCharactersData data)
    {
        string json = JsonUtility.ToJson(data);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { KEY, json }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Cloud Save Success"),
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    public static void Load(System.Action<PlayerCharactersData> onLoaded)
    {
        var request = new GetUserDataRequest();

        PlayFabClientAPI.GetUserData(request,
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("PLAYER_CHARACTERS"))
                {
                    string json = result.Data["PLAYER_CHARACTERS"].Value;
                    var data = JsonUtility.FromJson<PlayerCharactersData>(json);
                    onLoaded?.Invoke(data);
                }
                else
                {
                    onLoaded?.Invoke(null);
                }
            },
            error => Debug.LogError(error.GenerateErrorReport()));
    }
}