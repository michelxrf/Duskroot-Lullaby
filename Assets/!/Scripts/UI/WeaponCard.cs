using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a weapon card displayed in the UI.
/// Handles the visual display of weapon data including name, damage, knockback, and weapon model preview.
/// </summary>
public class WeaponCard : MonoBehaviour
{
    [SerializeField] TMP_Text weaponName;
    [SerializeField] TMP_Text damageLabel;
    [SerializeField] TMP_Text knockbackLabel;
    [SerializeField] Image weaponIcon;

    /// <summary>
    /// Initializes the weapon card with character and weapon data.
    /// </summary>
    /// <param name="character">The character data containing weapon information</param>
    public void Initialize(CharacterData character)
    {
        UpdateWeapon(character.weapon, character.weaponLevel);
    }

    /// <summary>
    /// Updates the weapon card display with the current weapon data.
    /// </summary>
    /// <param name="weapon">The weapon data to display</param>
    public void UpdateWeapon(WeaponData weapon, int weaponLevel)
    {
        if (weapon == null)
            return;

        weaponIcon.sprite = weapon.weaponPortrait[weaponLevel];
        weaponName.text = weapon.name;
        damageLabel.text = $"DMG: {weapon.baseDamage}";
        knockbackLabel.text = $"KB: {weapon.knockbackForce}";
    }
}
