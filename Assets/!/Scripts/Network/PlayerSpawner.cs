using UnityEngine;
using Fusion;


/// <summary>
/// Handles spawning a player character when a player joins the simulation.
/// </summary>
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if(player == Runner.LocalPlayer)
        {
            Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, Runner.LocalPlayer);
        }
    }
}
