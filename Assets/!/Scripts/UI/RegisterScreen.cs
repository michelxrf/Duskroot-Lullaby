using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public override void Show()
    {
        base.Show();

        usernameInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
    }

    void AllowInteractions(bool value)
    {
        registerButton.interactable = value;
        backButton.interactable = value;
        passwordInput.interactable = value;
        usernameInput.interactable = value;
        confirmPasswordInput.interactable = value;
    }

    void OnBackClicked()
    {
        uiManager.ShowScreen(loginScreen);
    }
    void OnRegisterClicked()
    {
        playFabAuth.Register(usernameInput.text, passwordInput.text, RegisterSuccessCallback, RegisterFailCallback);

        AllowInteractions(false);
    }

    void RegisterSuccessCallback()
    {
        AllowInteractions(true);

        PlayerPrefs.SetString("username", usernameInput.text);
        PlayerPrefs.Save();

        uiManager.ShowScreen(loginScreen);
    }

    void RegisterFailCallback()
    {
        AllowInteractions(true);
    }

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
