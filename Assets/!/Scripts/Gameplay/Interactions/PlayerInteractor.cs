using UnityEngine;
using Fusion;
using CombatSystem;

public class PlayerInteractor : NetworkBehaviour
{
    Interactions interactionInRange;
    ReviveTombstone reviveTombstoneInRange;
    PickableWeapon pickableWeaponInRange;

    public void EnteredInteractionArea(Interactions interaction)
    {
        interactionInRange = interaction;
    }

    public void LeftInteractionArea()
    {
        interactionInRange = null;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        // Get input data from the network
        if (GetInput(out NetworkInputData data))
        {
            if (data.Interact)
            {
                if (reviveTombstoneInRange != null)
                {
                    reviveTombstoneInRange.TryInteract();
                    return;
                }

                if (pickableWeaponInRange != null)
                {
                    EquipPickedUpWeapon();
                    return;
                }

                if (interactionInRange != null)
                {
                    Debug.Log("Interacted with " + interactionInRange.name);
                    interactionInRange.RPC_ActivateBark();
                }
            }
        }
    }

    private void EquipPickedUpWeapon()
    {
        if (pickableWeaponInRange == null)
            return;

        var weaponData = pickableWeaponInRange.PickupWeapon();
        GetComponent<PlayerAttack>()?.EquipWeapon(weaponData);
    }

    public void EnteredReviveArea(ReviveTombstone reviveTombstone)
    {
        reviveTombstoneInRange = reviveTombstone;
    }

    public void LeftReviveArea(ReviveTombstone reviveTombstone)
    {
        if (reviveTombstoneInRange == reviveTombstone)
            reviveTombstoneInRange = null;
    }

    public void EnteredPickableWeaponArea(PickableWeapon pickableWeapon)
    {
        pickableWeaponInRange = pickableWeapon;
    }

    public void LeftPickableWeaponArea(PickableWeapon pickableWeapon)
    {
        if (pickableWeaponInRange == pickableWeapon)
            pickableWeaponInRange = null;
    }
}
