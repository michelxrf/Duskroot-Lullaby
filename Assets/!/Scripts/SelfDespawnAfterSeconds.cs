using Fusion;
using UnityEngine;

public class SelfDespawnAfterSeconds : NetworkBehaviour
{
    [SerializeField] private float secondsToDespawn = 1f;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Invoke(nameof(Despawn), secondsToDespawn);
        }
    }

    private void Despawn()
    {
        if (Object != null && Runner != null)
        {
            Runner.Despawn(Object);
        }
    }
}
