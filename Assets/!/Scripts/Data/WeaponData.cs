using UnityEngine;

/// <summary>
/// Scriptable Object that contains all data an d configuration for a weapon.
/// Includes damage values, animations, models, and behavior type.
/// </summary>
[CreateAssetMenu(fileName = "Scritable Objects/New Weapon Data", menuName = "Scriptable Objects/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    /// <summary>The base damage value for this weapon</summary>
    public int baseDamage = 25;

    /// <summary>The animator controller containing attack animations</summary>
    public RuntimeAnimatorController animationController;

    /// <summary>The 3D model prefab to display when this weapon is equipped</summary>
    public GameObject weaponModel;

    /// <summary>The radius of the hitbox for detecting hits</summary>
    public float hitboxRadius = 0.5f;

    /// <summary>Visual effects prefab to play on hit</summary>
    public GameObject vfxPrefab;

    /// <summary>Whether this weapon is held in the right hand (true) or left hand (false)</summary>
    public bool rigthHanded = true;

    /// <summary>The name of the weapon behavior implementation to use (e.g., "Melee")</summary>
    public string behavior;

    /// <summary>
    /// Used by audio system to determine which sound effects to play when this weapon hits a target.
    /// </summary>
    public string weaponAudioType = "unarmed";
}