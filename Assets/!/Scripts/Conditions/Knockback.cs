using UnityEngine;
using Fusion;
using CombatSystem;
using UnityEngine.AI;
using System.Collections;
using Fusion.Addons.SimpleKCC;


/// <summary>
/// Used to apply knockback effect to player and enemies
/// </summary>
public class Knockback : NetworkBehaviour
{
    [SerializeField] float KNOCKBACK_DURATION = .25f;
    [SerializeField] float KNOCKBACK_FORCE_MULTIPLIER = 10f; // adjust damage multiplier to balance knockback strength

    float knockbackTimer = 0f;
    Vector3 direction;
    float force;

    enum EntityType
    {
        Player,
        Enemy
    }
    EntityType entityType;
    public override void Spawned()
    {
        if (GetComponent<PlayerSetup>() != null)
        {
            entityType = EntityType.Player;
        }
        else if (GetComponent<EnemySetup>() != null)
        {
            entityType = EntityType.Enemy;
        }
        else
        {
            Debug.LogError("Knockback component attached to an object that is neither Player nor Enemy.");
        }
    }

    public void RPC_ApplyKnockback(Vector3 p_direction, int damage)
    {
        force = damage * (KNOCKBACK_FORCE_MULTIPLIER / KNOCKBACK_DURATION);
        direction = p_direction;
        knockbackTimer = KNOCKBACK_DURATION;
        EnableMovement(false);
    }

    public override void FixedUpdateNetwork()
    {
        if(!HasInputAuthority) return;

        if (knockbackTimer <= 0f)
        {
            EnableMovement(true);
        }
        else
        {
            knockbackTimer -= Runner.DeltaTime;
            GetComponent<SimpleKCC>()?.Move(direction * force * Runner.DeltaTime);
        }
    }

    void EnableMovement(bool newState)
    {
        if (entityType == EntityType.Player)
        {
            GetComponent<PlayerMovement>().enabled = newState;
            GetComponent<PlayerAttack>().enabled = newState;
            GetComponent<CharacterLook>().enabled = newState;
        }
        else if (entityType == EntityType.Enemy)
        {
            GetComponent<NavMeshAgent>().enabled = newState;
            GetComponent<StateMachine>().enabled = newState;
        }
    }
}
