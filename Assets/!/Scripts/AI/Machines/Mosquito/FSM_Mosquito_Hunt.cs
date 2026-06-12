using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class FSM_Mosquito_Hunt : State
{
    [SerializeField] float attackRange = .02f;
    [SerializeField] State stateOnPlayerLost;
    [SerializeField] State stateOnPlayerReached;
    [SerializeField] float rotationSpeed = 60f;

    Animator animator;
    VisionCollider visionCollider;
    NavMeshAgent navAgent;
    EnemyFeedbacks feedbacks;

    Transform target;
    float distanceToTarget;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        animator = GetComponentInChildren<Animator>();
        stateMachine = GetComponent<StateMachine>();
        feedbacks = GetComponent<EnemyFeedbacks>();
    }

    public override void Exit()
    {
        
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayLostVFX()
    {
        feedbacks.Play(EnemyFeedbackEvent.LostTarget);
    }

    public override void Process()
    {
        UpdateTargetLocation();
        animator.SetFloat("Speed", navAgent.velocity.magnitude);
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
        RPC_PlayAlertVFX();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayAlertVFX()
    {
        feedbacks.Play(EnemyFeedbackEvent.Alert);
    }
}
