using UnityEngine;
using Fusion;


/// <summary>
/// Handles spawning a player character when a player joins the simulation.
/// </summary>
public class PlayerSpawner : SimulationBehaviour, ISceneLoadDone
{
    public GameObject playerPrefab;

    public void SceneLoadDone(in SceneLoadDoneArgs sceneInfo)
    {
        Debug.Log("Scene load done");
        if (Runner.IsSharedModeMasterClient)
        {
            SpawnPlayers(Runner);
        }
    }

    private void SpawnPlayers(NetworkRunner runner)
    {
        foreach (var player in runner.ActivePlayers)
        {
            Debug.Log($"Spawning player for {player}");
            Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, Runner.LocalPlayer);
        }
    }
}
