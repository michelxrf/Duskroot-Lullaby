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
    public Action<Transform> OnPlayerEntered;
    public Action OnPlayerLeft;

    List<Transform> playersInRange = new List<Transform>();

    public void SetRange(float range)
    {
        GetComponent<SphereCollider>().radius = range;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        Debug.Log("Player entered vision collider");

        // ignore dead players
        PlayerHealth health = other.transform.GetComponent<PlayerHealth>();
        if (health == null) return;
        if (health.IsDead())
            return;

        health.OnDied += TargetDied;

        playersInRange.Add(other.transform);
        OnPlayerEntered?.Invoke(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!HasStateAuthority) return;

        other.GetComponent<PlayerHealth>().OnDied -= TargetDied;
        playersInRange.Remove(other.transform);
        OnPlayerLeft?.Invoke();
    }

    public bool IsPlayerInRange()
    {
        return playersInRange.Count > 0;
    }

    public Transform GetClosestPlayer()
    {
        Transform closestPlayer = null;
        float closestDistance = float.MaxValue;
        foreach (var player in playersInRange)
        {
            float distance = (player.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }

    void TargetDied()
    {
        foreach (var player in playersInRange)
        {
            if (player.GetComponent<PlayerHealth>().IsDead())
            {
                playersInRange.Remove(player);
                break;
            }
        }

        OnPlayerLeft?.Invoke();
    }
}
