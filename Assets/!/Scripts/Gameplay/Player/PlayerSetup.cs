using CombatSystem;
using Fusion;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;


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
    [SerializeField] GameObject antonioBox;
    [SerializeField] Transform rigTransform;

    Animator animator;

    Dictionary<Renderer, bool> rendererStates = new Dictionary<Renderer, bool>();
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
            rigTransform.localScale = characterId == "Antônio" ? Vector3.one * 1.27f : Vector3.one;
            antonioBox.SetActive(characterId == "Antônio" ? true : false);

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

    public void EnablePlayerControls(bool newState)
    {
        RPC_EnablePlayer(newState);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_EnablePlayer(bool newState)
    {
        GetComponent<PlayerMovement>().enabled = newState;
        GetComponent<PlayerAttack>().enabled = newState;
        GetComponent<CharacterLook>().enabled = newState;
        GetComponent<PlayerInteractor>().enabled = newState;
        GetComponent<Knockback>().enabled = newState;
    }

    public void EnablePlayerControlsLocal(bool newState)
    {
        if (!HasStateAuthority)
            return;

        GetComponent<PlayerMovement>().enabled = newState;
        GetComponent<PlayerAttack>().enabled = newState;
        GetComponent<CharacterLook>().enabled = newState;
        GetComponent<PlayerInteractor>().enabled = newState;
        GetComponent<Knockback>().enabled = newState;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_EnablePlayerVisuals(bool newState)
    {
        if (newState)
            RestorePlayerVisuals();
        else
            HidePlayerVisuals();
    }

    void HidePlayerVisuals()
    {
        rendererStates.Clear();

        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            rendererStates[renderer] = renderer.enabled;
            renderer.enabled = false;
        }

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            rendererStates[renderer] = renderer.enabled;
            renderer.enabled = false;
        }

        animator.SetFloat("Speed", 0f);
    }

    void RestorePlayerVisuals()
    {
        if (rendererStates.Count == 0)
            return;

        foreach (var rendererState in rendererStates)
        {
            rendererState.Key.enabled = rendererState.Value;
        }
        rendererStates.Clear();
    }
}
