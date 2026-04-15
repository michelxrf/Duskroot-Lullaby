using Fusion;
using UnityEngine;


/// <summary>
/// Initializes player-specific settings upon spawning in a networked game.
/// </summary>
public class PlayerSetup : NetworkBehaviour
{
    Camera playerCamera;
    [Networked] public string characterId { get => default; set { } }
    [Networked] public string currentWeapon { get => default; set { } }
    [SerializeField] GameObject characterModel;

    public override void Spawned()
    {
        Debug.Log("Player Spawned");

        if (HasStateAuthority)
        {
            playerCamera = Camera.main;
            playerCamera.GetComponent<FlyCamera>().target = transform;
            characterId = CharacterDataManager.Instance.GetCurrentPlayerCharacter().characterId;
            
            // loads character skin
            RPC_LoadCharacterModel();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_LoadCharacterModel()
    {
        characterModel = CharacterDataManager.Instance.GetCharacterModel(characterId);
        if (characterModel == null)
            return;

        Instantiate(characterModel, transform);
    }

    /// <summary>
    /// True when this player instance is controlled by the local client, false otherwise.
    /// </summary>
    public bool IsLocalPlayer()
    {
        return HasStateAuthority;
    }

    public string GetCharacterId()
    {
        return characterId;
    }

    public string GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public void SetCurrentWeapon(string weaponName)
    {
        currentWeapon = weaponName;
    }
}
