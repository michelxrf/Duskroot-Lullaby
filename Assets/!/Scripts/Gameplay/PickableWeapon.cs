using CombatSystem;
using Fusion;
using UnityEngine;

/// <summary>
/// Manages pickable weapons in the world that players can interact with.
/// Displays tooltips when players are in range and equips the weapon on the player's PlayerAttack component
/// when the interact button is pressed. Synchronized across network using Photon Fusion.
/// </summary>
public class PickableWeapon : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] GameObject interactionTooltip;
    [SerializeField] WeaponData weaponData;

    int playersInRange;
    bool hasSpawned = false;

    public override void Spawned()
    {
        base.Spawned();
        hasSpawned = true;

        playersInRange = 0;
        interactionTooltip.SetActive(false);
    }

    /// <summary>
    /// Called when a player enters the pickup trigger zone.
    /// Increments player count and notifies the player of the interaction area.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerSetup>() == null)
            return;

        playersInRange++;
        // TOOD: conect with player interactor to show interaction prompt
    }

    /// <summary>
    /// Called when a player exits the pickup trigger zone.
    /// Decrements player count and notifies the player they've left the interaction area.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerSetup>() == null)
            return;

        playersInRange--;
        // TODO: conect with player interactor to hide interaction prompt
        //other.GetComponent<PlayerInteractor>()?.LeftInteractionArea();
    }

    /// <summary>
    /// Determines whether the pickup tooltip should be visible.
    /// Shows tooltip if a player is in range.
    /// </summary>
    void UpdateTooltip()
    {
        if (!hasSpawned)
            return;

        if (playersInRange >= 1)
        {
            interactionTooltip.SetActive(true);
        }
        else
        {
            interactionTooltip.SetActive(false);
        }
    }

    /// <summary>
    /// Called every frame to update the visibility of the pickup tooltip.
    /// </summary>
    private void Update()
    {
        UpdateTooltip();
    }

    public WeaponData PickupWeapon()
    { 
        Runner.Despawn(Object); // TODO: test if this will not despawn item before returning
        return weaponData;
    }
}
