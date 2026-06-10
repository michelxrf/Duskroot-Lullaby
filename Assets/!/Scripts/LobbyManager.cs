using System.Collections;
using Fusion;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Used by LobbySeats as a reference to find other seats
/// </summary>
public class LobbyManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] CountdownToStart countDownUi;
    [SerializeField] string gameplaySceneName;
    public LobbySeat[] lobbySeats;

    [Header("Settings")]
    [SerializeField] int countdownSeconds = 5;

    [Networked] public int CurrentCountdown { get; private set; }
    private bool countdownInProgress = false;

    private void Start()
    {
        RunnerBootstrap.Instance.SetMaxPlayers(lobbySeats.Length);
        RunnerBootstrap.Instance.StartSession();

        foreach (var seat in lobbySeats)
        {
            seat.OnReadyStateChanged += AllowGameStart;
        }

        // Navegation
        Invoke(nameof(SelectFirstAvailableSeat),0.5f);
        ValidateCurrentSelection();
        foreach (var seat in lobbySeats)
        {
            seat.OnReadyStateChanged += AllowGameStart;
            seat.OnSeatStateChanged += ValidateCurrentSelection;
        }
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

        int countReadyPlayers = 0;
        foreach (var seat in lobbySeats)
        {
            if (seat.IsReady)
                countReadyPlayers++;
        }

        if (Runner.SessionInfo.PlayerCount != countReadyPlayers)
            return false;

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
        if (AreAllPlayersReady())
        {
            if (Runner.IsSharedModeMasterClient)
            {
                RPC_StartCountdown();
            }
        }
        else
        {
            if (Runner.IsSharedModeMasterClient)
            {
                RPC_StopCountdown();
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartCountdown()
    {
        if (!countdownInProgress)
        {
            countdownInProgress = true;
            countDownUi.Show();

            if (Runner.IsSharedModeMasterClient)
            {
                StartCoroutine(StartGameCountdown(countdownSeconds));
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StopCountdown()
    {
        if (countdownInProgress)
        {
            countdownInProgress = false;
            countDownUi.Hide();
            StopAllCoroutines();
            Debug.Log("Game start countdown stopped.");
        }
    }

    IEnumerator StartGameCountdown(int seconds)
    {
        Debug.Log("All players are ready! Starting the game in " + seconds + " seconds...");
        countDownUi.Show();
        
        float elapsedTime = 0f;

        while(elapsedTime < seconds)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            CurrentCountdown = Mathf.CeilToInt(seconds - elapsedTime);
        }

        StartGame();
    }

    private void StartGame()
    {
        if (!Runner.IsSharedModeMasterClient) return;
        
        // Prevent player from joining after game has started
        Runner.SessionInfo.IsVisible = false;
        Runner.SessionInfo.IsOpen = false;

        Runner.LoadScene(gameplaySceneName);
        Debug.Log("Starting the game now!");
    }

    public void StopGameStartCountdown()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            RPC_StopCountdown();
        }
    }

    private void OnDestroy()
    {
        foreach (var seat in lobbySeats)
        {
            if (seat != null)
                seat.OnReadyStateChanged -= AllowGameStart;
        }
    }

    //Navagation Functions
    public void SelectFirstAvailableSeat()
    {
        foreach (var seat in lobbySeats)
        {
            if (seat == null || seat.Object == null || !seat.Object.IsValid)
                continue;

            if (!seat.IsEmpty)
                continue;

            Button button =
                seat.GetSelectButton();

            if (button == null)
                continue;

            if (!button.gameObject.activeInHierarchy)
                continue;

            if (!button.interactable)
                continue;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button.gameObject);

            return;
        }
    }
    private void ValidateCurrentSelection()
    {
        if (EventSystem.current == null)
            return;

        GameObject current =
            EventSystem.current.currentSelectedGameObject;

        // N�o existe sele��o
        if (current == null)
        {
            SelectFirstAvailableSeat();
            return;
        }

        Button button =
            current.GetComponent<Button>();

        // N�o � um bot�o
        if (button == null)
        {
            SelectFirstAvailableSeat();
            return;
        }

        // Ficou invis�vel
        if (!button.gameObject.activeInHierarchy)
        {
            SelectFirstAvailableSeat();
            return;
        }

        // Ficou desabilitado
        if (!button.interactable)
        {
            SelectFirstAvailableSeat();
        }
    }


}

