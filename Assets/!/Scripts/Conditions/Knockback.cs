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
    [SerializeField] float knockbackDuration = .25f;
    const float forceMultiplier = .5f;

    float knockbackTimer = 0f;
    Vector3 direction;
    float force;

    enum EntityType
    {
        Player,
        Enemy
    }
    EntityType entityType;
    SimpleKCC characterController;
    public override void Spawned()
    {
        if (GetComponent<PlayerSetup>() != null)
        {
            entityType = EntityType.Player;
            characterController = GetComponent<SimpleKCC>();
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ApplyKnockback(Vector3 p_direction, int knockbackForce)
    {
        force = knockbackForce * forceMultiplier;
        direction = p_direction;
        knockbackTimer = knockbackDuration;
        EnableMovement(false);
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (knockbackTimer <= 0f)
        {
            EnableMovement(true);
        }
        else
        {
            knockbackTimer -= Time.deltaTime;

            if (entityType == EntityType.Player && characterController != null)
            {
                characterController.Move(direction * force);
            }
            else
            {
                transform.position += direction * force * Runner.DeltaTime;
            }
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
