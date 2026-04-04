using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Defines the patrol behavior for the mosquito enemy
/// It'll randomly select a point within a defined radius around a center point and move there. If it detects the player, it'll switch to the hunt state.
/// </summary>
public class FSM_Mosquito_Patrol : State
{
    [SerializeField] Vector3 patrolCenter;
    [SerializeField] float patrolRange = 5f;
    [SerializeField] float stoppingDistance = .1f;
    [SerializeField] State stateOnPlayerFound;

    Animator animator;
    VisionCollider visionCollider;
    NavMeshAgent navAgent;
    
    Vector3 patrolDestination;
    float distanceToDestination;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        animator = GetComponentInChildren<Animator>();
        stateMachine = GetComponent<StateMachine>();

        if (patrolCenter == null)
        {
            patrolCenter = transform.position;
            Debug.LogWarning("Patrol center not set for " + gameObject.name + ". Defaulting to current position.");
        }
    }

    public override void Enter()
    {
        patrolDestination = GetNewPatrolDestination();

        navAgent.stoppingDistance = stoppingDistance;
        navAgent.SetDestination(patrolDestination);

        visionCollider.OnPlayerEntered += PlayerFound;
    }

    public override void Exit()
    {
    }

    public override void Process()
    {
        if (IsDestinationReached())
        {
            patrolDestination = GetNewPatrolDestination();
            navAgent.SetDestination(patrolDestination);
        }

        animator.SetFloat("Speed", navAgent.velocity.magnitude);
    }

    Vector3 GetNewPatrolDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle * patrolRange;
        Vector3 destination = patrolCenter + new Vector3(randomPoint.x, 0, randomPoint.y);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, patrolRange, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return patrolCenter; // Fallback to center if no valid point found
    }

    bool IsDestinationReached()
    {
        distanceToDestination = (patrolDestination - transform.position).magnitude;
        return (distanceToDestination < stoppingDistance);
    }

    void PlayerFound(Transform playerTransf)
    {
        stateMachine.ChangeState(stateOnPlayerFound);
    }
}
