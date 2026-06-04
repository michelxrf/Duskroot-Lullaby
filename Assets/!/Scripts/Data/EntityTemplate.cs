using UnityEngine;

public class EntityTemplate : ScriptableObject
{
    public string CharacterId;
    public int Level;
    public int health;
    public int armor;
    public int damage;
    public float speed;
    public float attackSpeed;
    public int cure;
    public WeaponData weapon;

    public int healthUpgradePerLevel;
    public int armorUpgradePerLevel;
    public int damageUpgradePerLevel;
    public float speedUpgradePerLevel;
    public float attackSpeedUpgradePerLevel;
    public int cureUpgradePerLevel;
}
