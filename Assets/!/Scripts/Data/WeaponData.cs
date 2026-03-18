using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "Scritable Objects/New Weapon Data", menuName = "Scriptable Objects/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    public int baseDamage = 25;
    public float cooldownTimeSeconds = 1f;
    public AnimatorController animationController;
    public GameObject weaponModel;
    public float hitboxRadius = 0.5f;
    public GameObject vfxPrefab;
    public bool rigthHanded = true;
}
