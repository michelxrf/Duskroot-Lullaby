using UnityEngine;

/// <summary>
/// Scriptable Object that contains all data an d configuration for a weapon.
/// Includes damage values, animations, models, and behavior type.
/// </summary>
[CreateAssetMenu(fileName = "Scritable Objects/New Weapon Data", menuName = "Scriptable Objects/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    public int baseDamage = 25;
    public float attackSpeed = 1.0f;
    public int knockbackForce = 10;
    public RuntimeAnimatorController animationController;
    public float hitboxRadius = 0.5f;
    public GameObject[] vfxPrefab;
    public string[] behavior;
    public GameObject[] projectilePrefab;
    public string[] weaponModelName;
    public Sprite[] weaponPortrait;

    public string weaponAudioType = "unarmed";
}

public class WeaponDataInstance
{
    public WeaponData weaponData;
    public int weaponLevel;
    public string weaponSeed;

    public int damage;
    public int knockbackForce;
    public float hitboxRadius;
    public float attackSpeed;

    public WeaponDataInstance(WeaponData data, int level, string seed)
    {
        weaponData = data;
        weaponLevel = level;
        weaponSeed = seed;

        LevelWeapon();
    }

    void LevelWeapon()
    {
        int damagePoints = 0;
        int knockbackPoints = 0;
        int hitboxPoints = 0;

        // Fallback if seed is missing
        string seedToUse = string.IsNullOrEmpty(weaponSeed) ? "1" : weaponSeed;
        Random.InitState(seedToUse.GetHashCode());
        // ...

        for (int i = 0; i < weaponLevel; i++)
        {
            int coinToss = Random.Range(0, 3);
            
            switch (coinToss)
            {
                case 0:
                    damagePoints++;
                    break;
                case 1:
                    knockbackPoints++;
                    break;
                case 2:
                    hitboxPoints++;
                    break;
                default:
                    throw new System.Exception("Invalid random value for weapon leveling");
            };
        }

        damage = weaponData.baseDamage + (damagePoints * 5);
        knockbackForce = weaponData.knockbackForce + (knockbackPoints * 2);
        hitboxRadius = weaponData.hitboxRadius + (hitboxPoints * 0.1f);
        attackSpeed = weaponData.attackSpeed;
    }
}