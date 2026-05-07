using UnityEngine;
using Fusion;
using CombatSystem;

public class PlayerInteractor : NetworkBehaviour
{
    Interactions interactionInRange;
    ReviveTombstone reviveTombstoneInRange;
    PickableWeapon pickableWeaponInRange;

    [SerializeField] GameObject weaponDropPrefab;
    bool interactionButtonPressed = false; 

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
                if (interactionButtonPressed)
                    return;

                interactionButtonPressed = true;

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
            else
            {
                interactionButtonPressed = false;
            }
        }
        
    }

    private void EquipPickedUpWeapon()
    {
        if (pickableWeaponInRange == null)
            return;

        WeaponData dropWeapon = GetComponent<PlayerAttack>().GetCurrentWeaponData();
        var weaponData = pickableWeaponInRange.PickupWeapon();
        GetComponent<PlayerAttack>()?.EquipWeapon(weaponData);
        
        if(dropWeapon == null)
            pickableWeaponInRange.RPC_DespawnWeapon();
        else
            pickableWeaponInRange.Initialize(dropWeapon);
    }

    void DropWeapon(WeaponData weaponData)
    {
        if (!Runner.IsSharedModeMasterClient)
            return;

        if (weaponData == null)
            return;

        Vector3 dropPosition = transform.position;
        NetworkObject obj = Runner.Spawn(weaponDropPrefab, dropPosition, Quaternion.identity);
        obj.GetComponent<PickableWeapon>().Initialize(weaponData);
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
