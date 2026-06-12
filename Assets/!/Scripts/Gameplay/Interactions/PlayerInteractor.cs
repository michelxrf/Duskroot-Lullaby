using UnityEngine;
using Fusion;
using CombatSystem;

public class PlayerInteractor : NetworkBehaviour
{
    Interactions interactionInRange;
    ReviveTombstone reviveTombstoneInRange;
    PickableWeapon pickableWeaponInRange;
    PickableKey pickableKeyInRange;
    HealingItem healingItemInRange;
    ReadableCard readableCardInRange;
    private GramophoneController currentGramophone;

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

    public void EnteredHealingItemArea(HealingItem item)
    {
        healingItemInRange = item;
    }

    public void LeftHealingItemArea(HealingItem item)
    {
        if (healingItemInRange == item)
            healingItemInRange = null;
    }

    public void EnteredReadableCardArea(ReadableCard card)
    {
        readableCardInRange = card;
    }

    public void LeftReadableCardArea(ReadableCard card)
    {
        if (readableCardInRange == card)
            readableCardInRange = null;
    }

    public void EnteredGramophoneArea(
    GramophoneController gramophone)
    {
        currentGramophone = gramophone;
    }
    public void LeftGramophoneArea()
    {
        currentGramophone = null;
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

                if (pickableKeyInRange != null)
                {
                    PickUpKey();
                    return;
                }

                if (healingItemInRange != null)
                {
                    healingItemInRange.Consume(GetComponent<PlayerHealth>());
                    return;
                }

                if (readableCardInRange != null)
                {
                    readableCardInRange.OpenCard();
                    return;
                }
                if (currentGramophone != null)
                {
                    currentGramophone.Interact();
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

        WeaponDataInstance dropWeapon = GetComponent<PlayerAttack>().GetCurrentWeaponData();

        WeaponDataInstance pickedWeaponData = pickableWeaponInRange.PickupWeapon();

        Debug.Log($"{pickedWeaponData.weaponData.name}");

        GetComponent<PlayerAttack>()?.EquipWeapon(pickedWeaponData.weaponData, pickedWeaponData.weaponLevel, pickedWeaponData.weaponSeed);
        
        if(dropWeapon == null)
            pickableWeaponInRange.RPC_DespawnWeapon();
        else
            pickableWeaponInRange.Initialize(dropWeapon.weaponData, dropWeapon.weaponLevel, dropWeapon.weaponSeed);
    }

    private void PickUpKey()
    {
        if (pickableKeyInRange == null)
            return;

        PlayerKeyInventory inventory = GetComponent<PlayerKeyInventory>();
        if (inventory != null)
        {
            inventory.AddKeys(pickableKeyInRange.GetKeys());
            pickableKeyInRange.RPC_Despawn();
        }
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

    public void EnteredPickableKeyArea(PickableKey pickableKey)
    {
        pickableKeyInRange = pickableKey;
    }

    public void LeftPickableKeyArea(PickableKey pickableKey)
    {
        if (pickableKeyInRange == pickableKey)
            pickableKeyInRange = null;
    }
}
