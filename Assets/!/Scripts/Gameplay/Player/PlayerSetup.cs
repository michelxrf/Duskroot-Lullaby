using Fusion;
using UnityEngine;


/// <summary>
/// Initializes player-specific settings upon spawning in a networked game.
/// </summary>
public class PlayerSetup : NetworkBehaviour
{
    Camera playerCamera;

    public override void Spawned()
    {
        Debug.Log("Player Spawned");

        if (HasStateAuthority)
        {
            playerCamera = Camera.main;
            playerCamera.GetComponent<FlyCamera>().target = transform;
        }
    }

    /// <summary>
    /// True when this player instance is controlled by the local client, false otherwise.
    /// </summary>
    public bool IsLocalPlayer()
    {
        return HasStateAuthority;
    }
}
