using UnityEngine;
using Fusion;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using CombatSystem;

/// <summary>
/// Used by AI to detect player within a range
/// </summary>
public class VisionCollider : NetworkBehaviour
{
    public Action<PlayerHealth> OnPlayerEntered;
    public Action OnPlayerLeft;

    List<PlayerHealth> playersInRange = new List<PlayerHealth>();

    public void SetRange(float range)
    {
        GetComponent<SphereCollider>().radius = range;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        // ignore dead players
        PlayerHealth health = other.transform.GetComponent<PlayerHealth>();
        if (health == null) return;
        if (health.IsDead())
            return;

        // also ignore invunerable ones
        if (health.IsInvulnerable)
            return;

        playersInRange.Add(other.GetComponent<PlayerHealth>());
        OnPlayerEntered?.Invoke(other.GetComponent<PlayerHealth>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!HasStateAuthority) return;

        playersInRange.Remove(other.GetComponent<PlayerHealth>());
        OnPlayerLeft?.Invoke();
    }

    public Transform GetClosestPlayer()
    {
        Transform closestPlayer = null;
        float closestDistance = float.MaxValue;
        foreach (var player in playersInRange)
        {
            if (player.IsInvulnerable || player.IsDead())
                continue;

            float distance = (player.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }
        return closestPlayer;
    }
}
