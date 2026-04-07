using System;
using UnityEngine;

public class AudioHitNotifier : MonoBehaviour
{
    [HideInInspector] public CHARACTERTYPES characterType;
    [HideInInspector] public WEAPONTYPES weaponType;

    public event Action<CHARACTERTYPES, WEAPONTYPES> OnHit;

    public void SetCharacterType(string characterType)
    {
        this.characterType = ParseCharacterType(characterType);
    }

    public WEAPONTYPES ParseWeaponType(string weaponType)
    {
        if (Enum.TryParse<WEAPONTYPES>(weaponType, true, out var result))
        {       
            return result;
        }
        return WEAPONTYPES.Unarmed;
    }

    public CHARACTERTYPES ParseCharacterType(string characterType)
    {
        if (Enum.TryParse<CHARACTERTYPES>(characterType, true, out var result))
        {
            return result;
        }
        return CHARACTERTYPES.Mage;
    }

    public void NotifyHit(string weaponType)
    {
        OnHit?.Invoke(characterType, ParseWeaponType(weaponType));
    }
}

public enum CHARACTERTYPES
{
    Mage,
    Ranger,
    Warrior,
    Tank
}

public enum WEAPONTYPES
{
    Unarmed,
    Club,
    Blade,
    Magic,
    Projectile,
    Fly
}