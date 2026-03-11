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
}
