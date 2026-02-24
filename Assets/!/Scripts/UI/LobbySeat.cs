using System;
using System.Runtime.Serialization.Formatters;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbySeat : MonoBehaviour
{
    LobbyManager lobbyManager;
    CharacterData characterData;
    string characterId;

    [Header("References")]
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text characterRole;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button leaveSeatButton;
    [SerializeField] private TMP_Text playerName;

    [HideInInspector] public bool IsReady { get; private set; } = false;
    [HideInInspector] public bool IsEmpty { get; private set; } = false;
    
    public Action OnReadyStateChanged;

    private void Start()
    {
        characterName.text = characterData.CharacterId;
    }

    public void OnReadyClicked()
    {
        IsReady = !IsReady;
        OnReadyStateChanged?.Invoke();
    }

    public void AssignPlayer(CharacterData data)
    {
        if(data == null)
        {
            IsEmpty = true;
            return;
        }

        if(PlayerPrefs.HasKey("username"))
        {
            playerName.text = PlayerPrefs.GetString("username");
        }
    }
    
}
