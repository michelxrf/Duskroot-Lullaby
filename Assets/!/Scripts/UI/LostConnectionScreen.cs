using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Pop up screen that appears when the connection to the lobby is lost. It allows the player to return to the main menu.
/// </summary>
public class LostConnectionScreen : MonoBehaviour
{
    private void Start()
    {
        GetComponentInChildren<Button>().onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        gameObject.SetActive(false);
    }
}
