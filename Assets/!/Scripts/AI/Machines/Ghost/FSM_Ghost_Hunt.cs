using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class FSM_Ghost_Hunt : State
{
    [SerializeField] float attackRange = .02f;
    [SerializeField] State stateOnPlayerLost;
    [SerializeField] State stateOnPlayerReached;
    [SerializeField] float rotationSpeed = 60f;

    VisionCollider visionCollider;
    NavMeshAgent navAgent;
    EnemySetup enemySetup;
    EnemyFeedbacks feedbacks;

    Transform target;
    float distanceToTarget;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        stateMachine = GetComponent<StateMachine>();
        enemySetup = GetComponent<EnemySetup>();
        feedbacks = GetComponent<EnemyFeedbacks>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayLostVFX()
    {
        feedbacks.Play(EnemyFeedbackEvent.LostTarget);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayAlertVFX()
    {
        feedbacks.Play(EnemyFeedbackEvent.Alert);
    }

    public override void Exit()
    {

    }

    public override void Process()
    {
        UpdateTargetLocation();
        RotateTowardPathNode();
    }

    void UpdateTargetLocation()
    {
        target = visionCollider.GetClosestPlayer();

        // if we lost sight of the player, switch back to patrol
        if (target == null)
        {
            RPC_PlayLostVFX();
            stateMachine.ChangeState(stateOnPlayerLost);
            return;
        }

        navAgent.SetDestination(target.position);

        distanceToTarget = (target.position - transform.position).magnitude;

        // player is close enough to attack
        if (distanceToTarget < attackRange)
        {
            stateMachine.ChangeState(stateOnPlayerReached);
            return;
        }
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

    public override void Enter()
    {
        navAgent.isStopped = false;
        navAgent.speed = enemySetup.GetEnemyData().speed;
        RPC_PlayAlertVFX();
    }
}
