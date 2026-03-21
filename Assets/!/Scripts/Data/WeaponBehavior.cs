using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    protected Animator animator;
    protected GameObject owner;
    protected Transform defaultTarget;
    protected WeaponData weaponData;

    public virtual void Execute()
    {
        animator.SetTrigger("Attack");
    }

    public virtual void ImpactFrame()
    {
    }

    public virtual void Initialize(Transform defaultTarget, Animator anim, GameObject owner, WeaponData weaponData)
    {
        animator = anim;
        this.weaponData = weaponData;
        this.owner = owner;
        this.defaultTarget = defaultTarget;
    }
}
