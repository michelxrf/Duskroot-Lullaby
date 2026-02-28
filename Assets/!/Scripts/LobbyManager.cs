using System.Collections.Generic;
using Fusion;

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
}

