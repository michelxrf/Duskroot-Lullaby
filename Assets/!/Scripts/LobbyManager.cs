using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;

/// <summary>
/// Used by LobbySeats as a reference to find other seats
/// </summary>
public class LobbyManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text countDownText;
    [SerializeField] CanvasGroup countDownCanvas;
    [SerializeField] string gameplaySceneName;
    public LobbySeat[] lobbySeats;

    [Header("Settings")]
    [SerializeField] int countdownSeconds = 5;

    private void Start()
    {
        RunnerBootstrap.Instance.SetMaxPlayers(lobbySeats.Length);
        RunnerBootstrap.Instance.StartSession();

        foreach (var seat in lobbySeats)
        {
            seat.OnReadyStateChanged += AllowGameStart;
        }

        StopGameStartCountdown();
    }

    /// <summary>
    /// Retrieves the lobby seat currently occupied by the specified player.
    /// </summary>
    public LobbySeat GetPlayerSeat(PlayerRef playerRef)
    {
        foreach (var seat in lobbySeats)
        {
            if (seat.OccupyingPlayer == playerRef)
                return seat;
        }
        return null;
    }

    /// <summary>
    /// Verify if all are ready
    /// </summary>
    /// <returns></returns>
    public bool AreAllPlayersReady()
    {
        if (LobbyIsEmpty())
            return false;

        foreach (var seat in lobbySeats)
        {
            if (!seat.IsEmpty && !seat.IsReady)
                return false;
        }
        return true;
    }

    bool LobbyIsEmpty()
    {
        foreach (var seat in lobbySeats)
        {
            if (!seat.IsEmpty)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Allows the game to start, or sets a timer to auto start
    /// </summary>
    public void AllowGameStart()
    {
        if(!Runner.IsSharedModeMasterClient) return;

        if(AreAllPlayersReady())
        {
            StartCoroutine(StartGameCountdown(countdownSeconds));
        }
        else
        {
            StopGameStartCountdown();
        }
    }

    IEnumerator StartGameCountdown(int seconds)
    {
        Debug.Log("All players are ready! Starting the game in " + seconds + " seconds...");
        countDownText.text = seconds.ToString();
        countDownCanvas.alpha = 1f;
        
        float elapsedTime = 0f;

        while(elapsedTime < seconds)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            countDownText.text = Mathf.CeilToInt(seconds - elapsedTime).ToString();
        }

        Runner.LoadScene(gameplaySceneName);
        Debug.Log("Starting the game now!");
    }

    public void StopGameStartCountdown()
    {
        countDownCanvas.alpha = 0f;
        StopAllCoroutines();
        Debug.Log("Game start countdown stopped.");
    }

    private void OnDestroy()
    {
        foreach (var seat in lobbySeats)
        {
            if (seat != null)
                seat.OnReadyStateChanged -= AllowGameStart;
        }
    }
}

