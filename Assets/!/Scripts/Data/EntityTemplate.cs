using UnityEngine;

public class EntityTemplate : ScriptableObject
{
    public string CharacterId;
    public int Level;
    public int health;
    public int armor;
    public int damage;
    public int speed;
    public int attackSpeed;
    public int cure;
    public WeaponData weapon;

    public int healthUpgradePerLevel;
    public int armorUpgradePerLevel;
    public int damageUpgradePerLevel;
    public int speedUpgradePerLevel;
    public int attackSpeedUpgradePerLevel;
    public int cureUpgradePerLevel;
}
