using UnityEngine;
using Fusion;
using CombatSystem;

/// <summary>
/// Initializes the HUD (Heads-Up Display) when a player character spawns.
/// Sets up the player card display with character data and health information.
/// </summary>
public class HudInitializer : NetworkBehaviour
{
    /// <summary>
    /// Called when the network object is spawned.
    /// Initializes the players bar UI with the current character's data.
    /// </summary>
    public override void Spawned()
    {
        if(!HasStateAuthority)
            return;

        PlayersBar playerBar = FindFirstObjectByType<PlayersBar>();
        
        if (playerBar != null )
            playerBar.Initialize(CharacterDataManager.Instance.GetCurrentPlayerCharacter(), GetComponent<Health>());
    }
}
