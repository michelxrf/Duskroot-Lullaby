using Fusion;
using Unity.Cinemachine;
using UnityEngine;


/// <summary>
/// Initializes player-specific settings upon spawning in a networked game.
/// </summary>
public class PlayerSetup : NetworkBehaviour
{
    Camera playerCamera;
    [SerializeField] Quaternion cameraRotation = Quaternion.Euler(45f, 0f, 0f);
    [Networked] public string characterId { get => default; set { } }
    [Networked] public string currentWeapon { get => default; set { } }
    
    [SerializeField] GameObject[] characterModels;

    Animator animator;

    public override void Spawned()
    {
        animator = GetComponent<Animator>();

        Debug.Log("Player Spawned");

        if (HasStateAuthority)
        {
            SetUpCinemachine();
            characterId = CharacterDataManager.Instance.GetCurrentPlayerCharacter().characterId;

            // loads character skin
            RPC_LoadCharacterModel();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_LoadCharacterModel()
    {
        foreach (var model in characterModels)
        {
            model.SetActive(model.name == characterId);
        }
        
        animator.avatar = CharacterDataManager.Instance.GetCharacterAvatar(characterId);
    }


    /// <summary>
    /// Configures the Cinemachine virtual camera to follow and look at the current object's transform.
    /// </summary>
    void SetUpCinemachine()
    {
        // Set up Cinemachine virtual camera to follow the player
        Camera camera = Camera.main;

        CinemachineCamera cinemachineVirtualCamera = FindFirstObjectByType<CinemachineCamera>();
        if (cinemachineVirtualCamera != null)
        {
            cinemachineVirtualCamera.Follow = transform;
            cinemachineVirtualCamera.LookAt = transform;
            cinemachineVirtualCamera.GetComponent<CinemachineFollow>().FollowOffset = new Vector3(0f, 10f, -10f);
        }
        camera.transform.parent.transform.rotation = cameraRotation;
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
