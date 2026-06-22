using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


/// <summary>
/// Handles the registration and login of users using PlayFab's authentication system.
/// </summary>
public class PlayFabAuth : MonoBehaviour
{
    bool isLoggedIn = false;

    /// <summary>
    /// Registers a new user account with the specified username and password.
    /// </summary>
    public void Register(string username, string password, Action success, Action fail)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request,
            result =>
            {
                success?.Invoke();
                isLoggedIn = true;
                Debug.Log("Registered successfully! PlayFabId: " + result.PlayFabId);
            },
            error =>
            {
                fail?.Invoke();
                isLoggedIn = false;
                Debug.LogError("Register error: " + error.GenerateErrorReport());
            });
    }

    /// <summary>
    /// Handles user login.
    /// </summary>
    public void Login(string username, string password, Action success, Action fail)
    {
        if (isLoggedIn)
            return;

        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request,
            result =>
            {
                success?.Invoke();
                isLoggedIn = true;
                Debug.Log("Logged in! PlayFabId: " + result.PlayFabId);
            },
            error =>
            {
                fail?.Invoke();
                isLoggedIn = false;
                Debug.LogError("Login error: " + error.GenerateErrorReport());
            });
    }

    public void ResetLoginState()
    {
        isLoggedIn = false;
    }

}
