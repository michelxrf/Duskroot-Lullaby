using UnityEngine;
using Fusion;


/// <summary>
/// Handles spawning a player character when a player joins the simulation.
/// </summary>
public class PlayerSpawner : SimulationBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        RunnerBootstrap.Instance.OnSceneLoaded += SpawnPlayer;
    }

    void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: playerPrefab is not assigned!");
            return;
        }

        var runner = RunnerBootstrap.Instance?.Runner;
        if (runner == null)
        {
            Debug.LogError("PlayerSpawner: Runner is null!");
            return;
        }

        //var runner = RunnerBootstrap.Instance.Runner;
        Debug.Log($"Spawning player for {runner.LocalPlayer.ToString()}");
        runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, runner.LocalPlayer);
    }

    private void OnDestroy()
    {
        RunnerBootstrap.Instance.OnSceneLoaded -= SpawnPlayer;
    }
}
