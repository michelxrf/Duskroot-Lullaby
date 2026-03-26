using UnityEngine;
using Fusion;
using System;

public class VisionCollider : NetworkBehaviour
{
    public Action<Transform> OnPlayerEntered;
    public Action OnPlayerLeft;

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        OnPlayerEntered?.Invoke(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!HasStateAuthority) return;

        OnPlayerLeft?.Invoke();
    }
}
