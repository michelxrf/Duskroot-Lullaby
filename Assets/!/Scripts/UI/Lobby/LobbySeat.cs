using System;
using Fusion;
using PlayFab.MultiplayerModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySeat : NetworkBehaviour
{
    [SerializeField] CharacterTemplate characterTemplate;

    LobbyManager lobbyManager;

    [Header("References")]
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text characterLevel;
    [SerializeField] Image readyIndicator;
    [SerializeField] Button readyButton;
    [SerializeField] Button leaveSeatButton;
    [SerializeField] Button selectButton;

    // Networked properties to sync state across all clients
    [Networked] public NetworkBool IsReady { get; set; } = false;
    [Networked] public NetworkBool IsEmpty { get; set; }
    [Networked] public PlayerRef OccupyingPlayer { get; set; }
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public int CharacterLevel { get; set; }

    public Action OnReadyStateChanged;

    private void Start()
    {
        characterName.text = characterTemplate.CharacterId;
        lobbyManager = FindFirstObjectByType<LobbyManager>();

        readyIndicator.enabled = false;
        readyButton.onClick.AddListener(OnReadyClicked);
        leaveSeatButton.onClick.AddListener(OnLeaveSeatClicked);
        selectButton.onClick.AddListener(OnSelectClicked);
    }

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            IsEmpty = true;
            IsReady = false;
            
            // Subscribe only on state authority to avoid redundant processing
            if (RunnerBootstrap.Instance != null)
            {
                RunnerBootstrap.Instance.OnPlayerDisconnected += OnPlayerLeft;
            }
        }
    }

    public override void Render()
    {
        base.Render();

        if (Object == null || !Object.IsValid)
            return;

        UpdateUI();
    }

    private void OnPlayerLeft(PlayerRef player)
    {
        // Only clear this specific seat if it's occupied by the disconnected player
        if (!IsEmpty && OccupyingPlayer == player)
        {
            ClearSeat();
        }
    }

    private void UpdateUI()
    {
        bool isLocalPlayerSeat = OccupyingPlayer == Runner.LocalPlayer && !IsEmpty;
        bool localPlayerHasAnySeat = HasPlayerAlreadySelectedSeat(Runner.LocalPlayer);

        if (IsEmpty)
        {
            ShowEmptySeatUI(!localPlayerHasAnySeat);
        }
        else
        {
            ShowOccupiedSeatUI(isLocalPlayerSeat);
        }
    }

    private void ShowEmptySeatUI(bool showSelectButton)
    {
        playerName.text = "";
        readyButton.gameObject.SetActive(false);
        leaveSeatButton.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        characterLevel.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(showSelectButton);
        readyIndicator.enabled = false;
    }

    private void ShowOccupiedSeatUI(bool isLocalPlayerSeat)
    {
        playerName.text = PlayerName.ToString();
        playerName.gameObject.SetActive(true);

        characterLevel.text = CharacterLevel.ToString();
        characterLevel.gameObject.SetActive(true);

        // Only show buttons if this is the local player's seat
        readyButton.gameObject.SetActive(isLocalPlayerSeat);
        leaveSeatButton.gameObject.SetActive(isLocalPlayerSeat);
        selectButton.gameObject.SetActive(false);

        // Show ready indicator based on IsReady state (visible to all players)
        readyIndicator.enabled = IsReady;
    }

    public void ClearSeat()
    {
        IsReady = false;

        IsEmpty = true;
        OccupyingPlayer = PlayerRef.None;
        PlayerName = "";
        CharacterLevel = 0;

        OnReadyStateChanged?.Invoke();
    }

    private void AssignPlayer(PlayerRef player, int userCharacterLevel, string userName)
    {
        IsEmpty = false;
        OccupyingPlayer = player;
        PlayerName = userName;
        CharacterLevel = userCharacterLevel;

        IsReady = false;
    }

    public void OnReadyClicked()
    {
        RPC_OnReadyClicked();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_OnReadyClicked(RpcInfo info = default)
    {
        IsReady = !IsReady;
        OnReadyStateChanged?.Invoke();
        Debug.Log($"Player {PlayerName} is now {(IsReady ? "Ready" : "Not Ready")}");
    }

    public void OnLeaveSeatClicked()
    {
        RPC_OnLeaveSeatClicked();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_OnLeaveSeatClicked(RpcInfo info = default)
    {
        if (HasStateAuthority)
        {
            ClearSeat();
        }
    }

    public void OnSelectClicked()
    {
        if (!IsEmpty) return; // redundant check, preventing networked issues

        // Check if player already has a seat
        if (HasPlayerAlreadySelectedSeat(Runner.LocalPlayer)) return;

        // Get local player data
        CharacterData localCharacterData = CharacterDataManager.Instance.GetCharacter(characterTemplate.CharacterId);
        string localPlayerName = PlayerPrefs.GetString("username");
        int localCharacterLevel = localCharacterData != null ? localCharacterData.Level : 1;

        RPC_OnSelectClicked(localPlayerName, localCharacterLevel);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_OnSelectClicked(string userName, int characterLevel, RpcInfo info = default)
    {
        if (!IsEmpty) return; // redundant check, preventing networked issues

        PlayerRef player = info.Source;

        // Check if player already has a seat
        if (HasPlayerAlreadySelectedSeat(player)) return;

        if (HasStateAuthority)
        {
            AssignPlayer(player, characterLevel, userName);
        }
    }

    private bool HasPlayerAlreadySelectedSeat(PlayerRef player)
    {
        if (lobbyManager == null || lobbyManager.lobbySeats == null) return false;

        foreach (var seat in lobbyManager.lobbySeats)
        {
            if (seat == null || seat.Object == null || !seat.Object.IsValid) continue;

            if (!seat.IsEmpty && seat.OccupyingPlayer == player && seat != this)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDestroy()
    {
        if (HasStateAuthority && RunnerBootstrap.Instance != null)
        {
            RunnerBootstrap.Instance.OnPlayerDisconnected -= OnPlayerLeft;
        }
    }
}

