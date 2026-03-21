using CombatSystem;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Unarmed : WeaponBehavior
{
    public override void Execute()
    {
        base.Execute();
    }

    public override void ImpactFrame()
    {
        base.ImpactFrame();
        CombatFuncs.CastHitBox(defaultTarget, owner, weaponData);
    }

    public override void Initialize(Transform defaultTarget, Animator anim, GameObject owner, WeaponData weaponData)
    {
        base.Initialize(defaultTarget, anim, owner, weaponData);
    }
}
