using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents the registration/signup screen for creating new user accounts.
/// Handles user account creation with PlayFab authentication.
/// </summary>
public class RegisterScreen : UiScreen
{
    [SerializeField] PlayFabAuth playFabAuth;

    [Header("UI Elements")]
    [SerializeField] Button registerButton;
    [SerializeField] Button backButton;
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_InputField confirmPasswordInput;

    [Header("Other Screens")]
    [SerializeField] UiScreen loginScreen;
    [SerializeField] UiScreen lobbyScreen;

    protected override void Start()
    {
        base.Start();

        registerButton.onClick.AddListener(OnRegisterClicked);
        backButton.onClick.AddListener(OnBackClicked);

        usernameInput.onValueChanged.AddListener(OnUsernameFieldChange);
        passwordInput.onValueChanged.AddListener(OnPasswordFieldChanged);
        confirmPasswordInput.onValueChanged.AddListener(OnConfirmPasswordFieldChanged);

        CanRegister();
    }

    /// <summary>
    /// Shows the registration screen and clears all input fields.
    /// </summary>
    public override void Show()
    {
        base.Show();

        usernameInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
    }

    /// <summary>
    /// Enables or disables user interaction with the register screen UI elements.
    /// </summary>
    /// <param name="value">True to enable interaction, false to disable</param>
    void AllowInteractions(bool value)
    {
        registerButton.interactable = value;
        backButton.interactable = value;
        passwordInput.interactable = value;
        usernameInput.interactable = value;
        confirmPasswordInput.interactable = value;
    }

    /// <summary>
    /// Handles the back button click to return to the login screen.
    /// </summary>
    void OnBackClicked()
    {
        uiManager.ShowScreen(loginScreen);
    }

    /// <summary>
    /// Handles the register button click to submit account creation.
    /// </summary>
    void OnRegisterClicked()
    {
        playFabAuth.Register(usernameInput.text, passwordInput.text, RegisterSuccessCallback, RegisterFailCallback);

        AllowInteractions(false);
    }

    /// <summary>
    /// Callback for successful account registration.
    /// Saves credentials and transitions to the login screen.
    /// </summary>
    void RegisterSuccessCallback()
    {
        AllowInteractions(true);

        PlayerPrefs.SetString("username", usernameInput.text);
        PlayerPrefs.Save();

        uiManager.ShowScreen(loginScreen);
    }

    /// <summary>
    /// Callback for failed account registration.
    /// Re-enables UI for another registration attempt.
    /// </summary>
    void RegisterFailCallback()
    {
        AllowInteractions(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(registerButton.gameObject);
    }

    /// <summary>
    /// Handles username input field changes and validates registration eligibility.
    /// </summary>
    /// <param name="value">The new username value</param>
    void OnUsernameFieldChange(string value)
    {
        CanRegister();
    }

    void OnPasswordFieldChanged(string value)
    {
        CanRegister();
    }

    void OnConfirmPasswordFieldChanged(string value)
    {
        CanRegister();
    }

    void CanRegister()
    { 
        if (usernameInput.text.Length > 4 && passwordInput.text.Length >= 6 && confirmPasswordInput.text == passwordInput.text)
        {
            registerButton.interactable = true;
        }
        else
        {
            registerButton.interactable = false;
        }
    }
}
