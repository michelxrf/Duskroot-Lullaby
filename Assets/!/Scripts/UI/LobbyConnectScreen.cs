using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Represents the user interface screen for connecting to a lobby by entering a lobby code.
/// </summary>
public class LobbyConnectScreen : UiScreen
{
    [Header("UI Elements")]
    [SerializeField] Button connectButton;
    [SerializeField] Button backButton;
    [SerializeField] TMP_InputField lobbyCodeInput;

    [Header("Other Screens")]
    [SerializeField] UiScreen loginScreen;

    protected override void Start()
    {
        base.Start();

        connectButton.onClick.AddListener(OnConnectClicked);
        backButton.onClick.AddListener(OnBackClicked);
        lobbyCodeInput.onValueChanged.AddListener(OnLobbyCodeFieldChanged);

        CanConnect();
    }

    void OnBackClicked()
    {
        uiManager.ShowScreen(loginScreen);
    }

    void OnConnectClicked()
    {
        SceneManager.LoadScene("Lobby");
    }

    void OnLobbyCodeFieldChanged(string value)
    {
        CanConnect();
    }

    void CanConnect()
    {
        connectButton.interactable = !string.IsNullOrEmpty(lobbyCodeInput.text);
    }
}
