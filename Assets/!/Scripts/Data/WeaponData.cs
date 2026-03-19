using CombatSystem;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.UI.GridLayoutGroup;

[CreateAssetMenu(fileName = "Scritable Objects/New Weapon Data", menuName = "Scriptable Objects/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject, IWeaponBehavior
{
    public int baseDamage = 25;
    public float cooldownTimeSeconds = 1f;
    public RuntimeAnimatorController animationController;
    public GameObject weaponModel;
    public float hitboxRadius = 0.5f;
    public GameObject vfxPrefab;
    public bool rigthHanded = true;

    Animator animator;
    GameObject owner;
    Transform defaultTarget;

    public void Execute()
    {
        animator.SetTrigger("Attack");
    }

    public void ImpactFrame()
    {
        CombatFuncs.CastHitBox(defaultTarget, owner, this);
    }

    public void Initialize(Transform defaultTarget, Animator anim, GameObject owner)
    {
        animator = anim;
        this.owner = owner;
        this.defaultTarget = defaultTarget;
    }

}