using UnityEngine;

/// <summary>
/// Factory class responsible for creating weapon behavior instances based on a behavior name.
/// Follows the Factory pattern to decouple weapon creation from usage.
/// </summary>
public static class WeaponBehaviorFactory
{
    /// <summary>
    /// Creates a weapon behavior component of the specified type and attaches it to the target GameObject.
    /// </summary>
    /// <param name="behaviorName">The name of the behavior type (e.g., "Melee")</param>
    /// <param name="target">The GameObject to attach the behavior component to</param>
    /// <returns>The created WeaponBehavior instance, or null if behavior type is unknown</returns>
    public static WeaponBehavior CreateBehavior(string behaviorName, GameObject target)
    {
        return behaviorName switch
        {
            "Melee" => target.AddComponent<Melee>(),
            "Ranged" => target.AddComponent<Ranged>(),
            _ => null
        };

    }
}