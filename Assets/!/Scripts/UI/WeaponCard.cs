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
    /// Updates the weapon card display with the current weapon data.
    /// </summary>
    /// <param name="weapon">The weapon data to display</param>
    public void UpdateWeapon(WeaponDataInstance weapon)
    {
        if (weapon == null)
            return;

        int weaponLevel = weapon.weaponLevel;
        WeaponData weaponSO = weapon.weaponData;

        weaponIcon.sprite = weaponSO.weaponPortrait[weaponLevel];
        weaponName.text = weaponSO.name;
        damageLabel.text = $"DMG: {weapon.damage}";
        knockbackLabel.text = $"KB: {weapon.knockbackForce}";
    }
}
