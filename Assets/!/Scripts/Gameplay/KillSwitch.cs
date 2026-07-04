using CombatSystem;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class KillSwitch : NetworkBehaviour
{
    public UnityEvent ExecuteOnKill;

    Health health;

    public override void Spawned()
    {
        base.Spawned();
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDied += () =>
            {
                ExecuteOnKill?.Invoke();
            };
        }
    }
}
