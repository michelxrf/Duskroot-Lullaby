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
    [SerializeField] private Button randomLobbyButton;

    [Header("Other Screens")]
    [SerializeField] UiScreen loginScreen;
    [SerializeField] string lobby = "Lobby";


    protected override void Start()
    {
        randomLobbyButton.onClick.AddListener(OnCreateRandomLobbyClicked);
        base.Start();

        connectButton.onClick.AddListener(OnConnectClicked);
        backButton.onClick.AddListener(OnBackClicked);
        lobbyCodeInput.onValueChanged.AddListener(OnLobbyCodeFieldChanged);

        CanConnect();
    }

    void OnBackClicked()
    {
        uiManager.ShowScreen(loginScreen);
        FindFirstObjectByType<MainMenuCameraPositioner>().MoveToLeftAngle();
    }

    void OnConnectClicked()
    {
        RunnerBootstrap.Instance.SetSessionName(lobbyCodeInput.text);
        SceneManager.LoadScene(lobby);
    }

    void OnLobbyCodeFieldChanged(string value)
    {
        CanConnect();
    }

    void CanConnect()
    {
        connectButton.interactable = !string.IsNullOrEmpty(lobbyCodeInput.text);
    }

    public void OnCreateRandomLobbyClicked()
    {
        string roomName = GenerateRandomLobbyName();

        RunnerBootstrap.Instance.SetSessionName(roomName);

        SceneManager.LoadScene(lobby);
    }

    private string GenerateRandomLobbyName()
    {
        return $"Random{UnityEngine.Random.Range(0, 1000000):D6}";
    }
}
