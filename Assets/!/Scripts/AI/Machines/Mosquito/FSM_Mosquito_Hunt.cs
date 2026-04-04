using UnityEngine;
using UnityEngine.AI;

public class FSM_Mosquito_Hunt : State
{
    [SerializeField] float attackRange = .02f;
    [SerializeField] State stateOnPlayerLost;
    [SerializeField] State stateOnPlayerReached;

    Animator animator;
    VisionCollider visionCollider;
    NavMeshAgent navAgent;

    Transform target;
    float distanceToTarget;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        animator = GetComponentInChildren<Animator>();
        stateMachine = GetComponent<StateMachine>();
    }

    public override void Exit()
    {

    }

    public override void Process()
    {
        UpdateTargetLocation();
        animator.SetFloat("Speed", navAgent.velocity.magnitude);
    }

    void UpdateTargetLocation()
    {
        target = visionCollider.GetClosestPlayer();

        // if we lost sight of the player, switch back to patrol
        if (target == null)
        {
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

    public override void Enter()
    {
        
    }
}
