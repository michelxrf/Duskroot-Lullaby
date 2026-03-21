using UnityEngine;

public static class WeaponBehaviorFactory
{
    public static WeaponBehavior CreateBehavior(string behaviorName, GameObject target)
    {
        return behaviorName switch
        {
            "Unarmed" => target.AddComponent<Unarmed>(),
            _ => null
        };
    }
}