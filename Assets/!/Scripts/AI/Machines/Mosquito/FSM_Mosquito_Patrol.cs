using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Defines the patrol behavior for the mosquito enemy
/// It'll randomly select a point within a defined radius around a center point and move there. If it detects the player, it'll switch to the hunt state.
/// </summary>
public class FSM_Mosquito_Patrol : State
{
    [SerializeField] Transform patrolAnchor;
    [SerializeField] float patrolRange = 5f;
    [SerializeField] float stoppingDistance = .1f;
    [SerializeField] State stateOnPlayerFound;
    [SerializeField] float rotationSpeed = 60f;

    Animator animator;
    VisionCollider visionCollider;
    NavMeshAgent navAgent;
    EnemySetup enemySetup;
    EnemyData enemyData;

    Vector3 patrolDestination;
    float distanceToDestination;

    public override void Spawned()
    {
        navAgent = GetComponent<NavMeshAgent>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        animator = GetComponentInChildren<Animator>();
        stateMachine = GetComponent<StateMachine>();
        enemySetup = GetComponent<EnemySetup>();

        navAgent.updateRotation = false;
    }
    
    public override void Enter()
    {
        if (!enemySetup.IsInitialized())
        {
            enemySetup.OnInit += () =>
            {
                enemyData = enemySetup.GetEnemyData();
                navAgent.speed = enemyData.speed;
            };
        }

        patrolDestination = GetNewPatrolDestination();

        navAgent.stoppingDistance = stoppingDistance;
        navAgent.SetDestination(patrolDestination);

        visionCollider.OnPlayerEntered += PlayerFound;
    }

    public override void Exit()
    {
    }

void RotateTowardPathNode()
    {
        if (navAgent.hasPath && navAgent.path.corners.Length > 1)
        {
            Vector3 directionToNextCorner = (navAgent.path.corners[1] - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToNextCorner);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public override void Process()
    {
        if (IsDestinationReached())
        {
            patrolDestination = GetNewPatrolDestination();
            navAgent.SetDestination(patrolDestination);
        }

        RotateTowardPathNode();
        animator.SetFloat("Speed", navAgent.velocity.magnitude);
    }

    Vector3 GetNewPatrolDestination()
    {
        if (patrolAnchor == null)
        {
            patrolAnchor = transform.parent;
            if (patrolAnchor == null)
            {
                GameObject anchorObj = new GameObject(gameObject.name + "_PatrolAnchor");
                anchorObj.transform.position = transform.position;
                patrolAnchor = anchorObj.transform;

                Debug.LogWarning("Patrol anchor not set for " + gameObject.name + ". Defaulting to current position.");
            }
        }

        Vector2 randomPoint = Random.insideUnitCircle * patrolRange;
        Vector3 destination = patrolAnchor.position + new Vector3(randomPoint.x, 0, randomPoint.y);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, patrolRange, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return patrolAnchor.position; // Fallback to center if no valid point found
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
