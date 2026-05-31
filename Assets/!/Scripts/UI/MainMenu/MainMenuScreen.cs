using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen :
    UiScreen
{
    [Header("UI")]
    [SerializeField] Button playButton;
    [SerializeField] Button logoutButton;

    [Header("Screens")]
    [SerializeField] UiScreen enterRoomScreen;
    [SerializeField] UiScreen loginScreen;

    protected override void Start()
    {
        base.Start();
        playButton.onClick.AddListener(OnPlayClicked);

        logoutButton.onClick
            .AddListener(OnLogoutClicked);
    }
    private void OnPlayClicked()
    {
        uiManager.ShowScreen(enterRoomScreen);
    }
    private void OnLogoutClicked()
    {
        SessionManager.Instance.Logout();

        uiManager.ShowScreen(
            loginScreen
        );
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}