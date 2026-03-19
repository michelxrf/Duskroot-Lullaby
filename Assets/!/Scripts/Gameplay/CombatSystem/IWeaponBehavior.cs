using UnityEngine;

public interface IWeaponBehavior
{
    public abstract void Initialize(Transform defaultTarget, Animator anim, GameObject owner);
    public abstract void Execute();
}
