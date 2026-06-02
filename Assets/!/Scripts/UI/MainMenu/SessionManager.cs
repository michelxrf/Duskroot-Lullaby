using UnityEngine;

public class SessionManager : MonoBehaviour
{

    public static SessionManager Instance;

    private const string UsernameKey = "username";
    private const string PasswordKey = "password";
    private const string KeepLoggedKey = "keepLogged";

    [Header("UI")]
    [SerializeField] private UiManager uiManager;
    [SerializeField] private UiScreen loginScreen;
    [SerializeField] private UiScreen mainMenuScreen;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (uiManager == null)
            return;

        if (ShouldAutoLogin())
        {
            uiManager.ShowScreen(mainMenuScreen);
        }
        else
        {
            uiManager.ShowScreen(loginScreen);
        }
    }

    public void SaveSession(
        string username,
        string password,
        bool keepLogged)
    {
        PlayerPrefs.SetInt(
            KeepLoggedKey,
            keepLogged ? 1 : 0
        );

        if (keepLogged)
        {
            PlayerPrefs.SetString(
                UsernameKey,
                username
            );

            PlayerPrefs.SetString(
                PasswordKey,
                password
            );
        }
        else
        {
            PlayerPrefs.DeleteKey(
                PasswordKey
            );
        }

        PlayerPrefs.Save();
    }

    public bool ShouldAutoLogin()
    {
        return PlayerPrefs.GetInt(
            KeepLoggedKey,
            0
        ) == 1;
    }

    public string GetUsername()
    {
        return PlayerPrefs.GetString(
            UsernameKey,
            ""
        );
    }

    public string GetPassword()
    {
        return PlayerPrefs.GetString(
            PasswordKey,
            ""
        );
    }

    public void Logout()
    {
   
        PlayerPrefs.DeleteKey(
            "password"
        );

        PlayerPrefs.DeleteKey(
            "keepLogged"
        );

        PlayerPrefs.Save();
    }
}