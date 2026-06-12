using CombatSystem;
using Fusion;
using PlayFab.EconomyModels;
using TMPro;
using UnityEngine;
using static AudioPunchPlayer;

/// <summary>
/// Manages pickable weapons in the world that players can interact with.
/// Displays tooltips when players are in range and equips the weapon on the player's PlayerAttack component
/// when the interact button is pressed. Synchronized across network using Photon Fusion.
/// </summary>
public class PickableWeapon : NetworkBehaviour
{
    [Header("Setup")]
    [Tooltip("Will ignore set weapon and randomly select any weapon from all weapons SOs. Will ignore weaponDataSO if ticked.")]
    [SerializeField] bool randomizeWeapon = false;
    [Tooltip("Will randomly set the weapon's level, rarity is the default. Will ignore level var if ticked.")]
    [SerializeField] bool randomizeLevel = false;
    [SerializeField] WeaponData weaponDataSO;
    [SerializeField] int level = 0;
    
    [Header("References")]
    [SerializeField] GameObject interactionTooltip;
    [SerializeField] GameObject weaponStats;

    [SerializeField] TMP_Text tooltipWeaponName;
    [SerializeField] TMP_Text tooltipWeaponRarity;
    [SerializeField] TMP_Text tooltipWeaponDamage;
    [SerializeField] TMP_Text tooltipWeaponKnockback;
    [SerializeField] UnityEngine.UI.Image tooltipWeaponImage;

    private WeaponRarityFeedback rarityFeedback;

    [SerializeField] GameObject[] weaponModels;
    [SerializeField] GameObject pickableWeaponPrefab;

    string rarityTextColor;
    WeaponDataInstance weaponData;
    bool hasSpawned = false;

    public override void Spawned()
    {
        rarityFeedback = GetComponent<WeaponRarityFeedback>();

        base.Spawned();
        hasSpawned = true;

        interactionTooltip.SetActive(false);

        if(weaponDataSO != null)
        {
            if (randomizeWeapon)
                Debug.Log("Not implemented yet");

            if (randomizeLevel)
                Debug.Log("Not implemented yet");

            Initialize(weaponDataSO, level, Random.Range(1000000, 9999999).ToString());
            InitializeWeaponModel();
            SetupStatsTooltip();
        }
    }

    public void Initialize(WeaponData newWeaponData, int weaponLevel, string weaponSeed)
    {
        weaponData = new WeaponDataInstance(newWeaponData, weaponLevel, weaponSeed);
        RPC_InitializeWeapon(newWeaponData.name, weaponLevel, weaponSeed);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_InitializeWeapon(string weaponName, int weaponLevel, string weaponSeed)
    {
        weaponData = new WeaponDataInstance(Resources.Load<WeaponData>($"Data/Weapons/Player/{weaponName}"), weaponLevel, weaponSeed);
        InitializeWeaponModel();
        SetupStatsTooltip();
    }
    
    void SetupStatsTooltip()
    {   
        tooltipWeaponName.text = weaponData.weaponData.name;
        tooltipWeaponDamage.text = $"+ {weaponData.damage}";
        tooltipWeaponKnockback.text = $"+ {weaponData.knockbackForce}";
        //tooltipWeaponImage;
        switch (weaponData.weaponLevel)
        {
            case 0:
                tooltipWeaponRarity.text = "Comum";
                rarityTextColor = "20872D"; 
                break;
            case 1:
                tooltipWeaponRarity.text = "Incomun";
                rarityTextColor = "326BC8";
                break;
            case 2:
                tooltipWeaponRarity.text = "Rara";
                rarityTextColor = "C232C8";
                break;
            case 3:
                tooltipWeaponRarity.text = "Lendária";
                rarityTextColor = "C19C10";
                break;
            default:
                tooltipWeaponRarity.text = "Irreconhcível";

                break;

        }
        // Converte HEX em Color
        if (ColorUtility.TryParseHtmlString("#" + rarityTextColor, out Color rarityColor))
        {
            tooltipWeaponRarity.color = rarityColor;
        }

        // Define portrait da arma
        tooltipWeaponImage.sprite = weaponData.weaponData.weaponPortrait[weaponData.weaponLevel];

        //Feedback da raridade da arma
        rarityFeedback?.PlayRarityFeedback(weaponData.weaponLevel);
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
            model.SetActive(model.name == weaponData.weaponData.weaponModelName[weaponData.weaponLevel]);
        }
    }

    /// <summary>
    /// Called when a player enters the pickup trigger zone.
    /// Increments player count and notifies the player of the interaction area.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        interactionTooltip.SetActive(true);
        other.GetComponent<PlayerInteractor>()?.EnteredPickableWeaponArea(this);
    }

    /// <summary>
    /// Called when a player exits the pickup trigger zone.
    /// Decrements player count and notifies the player they've left the interaction area.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        interactionTooltip.SetActive(false);
        other.GetComponent<PlayerInteractor>()?.LeftPickableWeaponArea(this);
    }

    public WeaponDataInstance PickupWeapon()
    {
        AudioUI.instance.PlayUIWeapon();
        return weaponData;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DespawnWeapon()
    {
        Runner.Despawn(Object);
    }
}
