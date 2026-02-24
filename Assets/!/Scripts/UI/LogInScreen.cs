using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Represents the login screen for user authentication
/// </summary>
public class LogInScreen : UiScreen
{
    [SerializeField] PlayFabAuth playFabAuth;

    [Header("UI Elements")]
    [SerializeField] Button loginButton;
    [SerializeField] Button registerButton;
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;

    [Header("Other Screens")]
    [SerializeField] UiScreen registerScreen;
    [SerializeField] UiScreen lobbyScreen;


    protected override void Start()
    {
        base.Start();

        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        usernameInput.onValueChanged.AddListener(OnUsernameFieldChange);
        passwordInput.onValueChanged.AddListener(OnPasswordFieldChanged);

        CanLogin();
    }

    /// <summary>
    /// Shows the login screen
    /// </summary>
    public override void Show()
    {
        base.Show();
        
        passwordInput.text = "";

        // Load saved credentials if they exist
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            CanLogin();
        }
    }

    void OnLoginClicked()
    {
        playFabAuth.Login(usernameInput.text, passwordInput.text, LoginSucessCallback, LoginFailCallback);
        AllowInteractions(false);
    }

    /// <summary>
    /// Enables or disables user interaction with the login ui
    /// </summary>
    void AllowInteractions(bool value)
    {
        loginButton.interactable = value;
        registerButton.interactable = value;
        passwordInput.interactable = value;
        usernameInput.interactable = value;
    }

    /// <summary>
    /// Reacts to a successful login, saving username, requesting user data from PlayFab, and transitioning to the lobby screen
    /// </summary>
    void LoginSucessCallback()
    {
        // Transition to the lobby screen on successful login
        uiManager.ShowScreen(lobbyScreen);

        // Save the username for future sessions
        PlayerPrefs.SetString("username", usernameInput.text);
        PlayerPrefs.Save();

        // wait for character data from PlayFab to load before allowing player to continue to lobby
        PlayFabCharacterSave.Load((data) =>
            {
                CharacterDataManager.Instance.Initialize(data);
                Debug.Log("Character data loaded successfully.");
                
                // Re-enable interactions in case the user logs out and returns to this screen
                AllowInteractions(true);
            }
        );
    }

    /// <summary>
    /// Reaction to a failed login attempt, re-enabling user interaction with the login ui
    /// TODO: Add error message display to the user in the future
    /// </summary>
    void LoginFailCallback()
    {
        AllowInteractions(true);
    }

    void OnRegisterButtonClicked()
    {
        uiManager.ShowScreen(registerScreen);
    }

    void OnUsernameFieldChange(string value)
    {
        CanLogin();
    }

    void OnPasswordFieldChanged(string value)
    {
        CanLogin();
    }

    /// <summary>
    /// Verifies if inputs meet the minimum requirements for a login attempt
    /// </summary>
    void CanLogin()
    {         
        if (usernameInput.text.Length > 4 && passwordInput.text.Length >= 6)
        {
            loginButton.interactable = true;
        }
        else
        {
            loginButton.interactable = false;
        }
    }
}
