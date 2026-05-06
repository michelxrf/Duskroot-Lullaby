using CombatSystem;
using Fusion;
using TMPro;
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
    [SerializeField] GameObject weaponStats;
    [SerializeField] WeaponData weaponData;

    [SerializeField] TMP_Text tooltipWeaponName;
    [SerializeField] TMP_Text tooltipWeaponDamage;
    [SerializeField] TMP_Text tooltipWeaponForce;

    [SerializeField] GameObject[] weaponModels;

    int playersInRange;
    bool hasSpawned = false;

    public override void Spawned()
    {
        base.Spawned();
        hasSpawned = true;

        playersInRange = 0;
        interactionTooltip.SetActive(false);

        if(weaponData != null)
        {
            InitializeWeaponModel();
            SetupStatsTooltip();
        }
    }

    public void Initialize(WeaponData newWeaponData)
    {
        weaponData = newWeaponData;
        if (HasInputAuthority)
        {
            RPC_InitializeWeapon(newWeaponData.name);
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_InitializeWeapon(string weaponName)
    {
        weaponData = Resources.Load<WeaponData>($"Data/Weapons/Player/{weaponName}");
        InitializeWeaponModel();
        SetupStatsTooltip();
    }
    
    void SetupStatsTooltip()
    {
        tooltipWeaponName.text = weaponData.name;
        tooltipWeaponDamage.text = $"Damage: {weaponData.baseDamage}";
        tooltipWeaponForce.text = $"Force: {weaponData.knockbackForce}";
    }

    /// <summary>
    /// Initializes the weapon model display based on the weapon data.
    /// Activates the correct weapon model and deactivates others.
    /// </summary>
    void InitializeWeaponModel()
    {
        if (weaponData == null || weaponModels.Length == 0)
            return;

        foreach (var model in weaponModels)
        {
            model.SetActive(model.name == weaponData.weaponModelName);
        }
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
        other.GetComponent<PlayerInteractor>()?.EnteredPickableWeaponArea(this);
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
        other.GetComponent<PlayerInteractor>()?.LeftPickableWeaponArea(this);
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
        RPC_DespawnWeapon();
        return weaponData;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_DespawnWeapon()
    {
        Runner.Despawn(Object);
    }
}
