using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Provides functionality to initialize and manage a Fusion NetworkRunner instance, handle network session lifecycle,
/// and respond to network events within a Unity game.
/// </summary>
public class RunnerBootstrap : MonoBehaviour, INetworkRunnerCallbacks
{
    public static RunnerBootstrap Instance { get; private set; }

    public NetworkRunner Runner { get; private set; }
    public string SessionName { get; private set; } = "MyLobby";
    public int MaxPlayers { get; private set; } = 1;

    // Actions
    public Action OnConnected;
    public Action OnPlayerConnected;
    public Action<PlayerRef> OnPlayerDisconnected;
    public Action OnFailedToConnect;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // required so that the application doesn't pause when the user tabs out of the game window
        Application.runInBackground = true;

        Runner = GetComponent<NetworkRunner>();
        Runner.ProvideInput = true;

        Runner.AddCallbacks(this);
    }

    public void SetSessionName(string sessionName)
    {
        SessionName = sessionName;
    }

    public void SetMaxPlayers(int maxPlayers)
    {
        MaxPlayers = maxPlayers;
    }

    public async void StartSession()
    {
        if (Runner.IsRunning)
            return;

        await Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = SessionName,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = GetComponent<NetworkSceneManagerDefault>(),
            PlayerCount = MaxPlayers
        });
    }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { OnPlayerConnected?.Invoke(); }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { OnPlayerDisconnected?.Invoke(player); }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { { OnFailedToConnect?.Invoke(); } }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { OnFailedToConnect?.Invoke(); }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { OnFailedToConnect?.Invoke(); }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { OnConnected?.Invoke(); }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
}
