using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    void AllowInteractions(bool value)
    {
        loginButton.interactable = value;
        registerButton.interactable = value;
        passwordInput.interactable = value;
        usernameInput.interactable = value;
    }

    void LoginSucessCallback()
    {
        uiManager.ShowScreen(lobbyScreen);

        PlayerPrefs.SetString("username", usernameInput.text);
        PlayerPrefs.SetString("password", passwordInput.text);
        PlayerPrefs.Save();

        AllowInteractions(true);
    }

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
