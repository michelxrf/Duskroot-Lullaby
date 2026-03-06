using Fusion;
using UnityEngine;

/// <summary>
/// Used by LobbySeats as a reference to find other seats
/// </summary>
public class LobbyManager : NetworkBehaviour
{
    public LobbySeat[] lobbySeats;

    private void Start()
    {
        RunnerBootstrap.Instance.SetMaxPlayers(lobbySeats.Length);
        RunnerBootstrap.Instance.StartSession();

        foreach (var seat in lobbySeats)
        {
            seat.OnReadyStateChanged += AllowGameStart;
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
        if(!HasStateAuthority) return;

        if(AreAllPlayersReady())
            Debug.Log("All players are ready! Starting the game...");
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

