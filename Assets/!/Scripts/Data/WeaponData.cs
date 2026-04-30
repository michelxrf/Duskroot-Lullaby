using UnityEngine;

/// <summary>
/// Scriptable Object that contains all data an d configuration for a weapon.
/// Includes damage values, animations, models, and behavior type.
/// </summary>
[CreateAssetMenu(fileName = "Scritable Objects/New Weapon Data", menuName = "Scriptable Objects/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    public int baseDamage = 25;
    public int knockbackForce = 10;
    public RuntimeAnimatorController animationController;
    public GameObject weaponModel;
    public float hitboxRadius = 0.5f;
    public GameObject vfxPrefab;
    public bool rigthHanded = true;
    public string behavior;
    public GameObject projectilePrefab;
    public string weaponModelName;

    public string weaponAudioType = "unarmed";
}