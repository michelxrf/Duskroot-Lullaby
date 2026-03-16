using TMPro;
using UnityEngine;

/// <summary>
/// Shows the countdown based on LobbyManager countdown to start the game
/// </summary>
public class CountdownToStart : MonoBehaviour
{
    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] TMP_Text countdownText;

    CanvasGroup canvasGroup;
    bool active = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    private void Update()
    {
        if (active)
            countdownText.text = lobbyManager.CurrentCountdown.ToString();
    }

    public void Show()
    {
        active = true;
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        active = false;
        canvasGroup.alpha = 0;
    }


}
