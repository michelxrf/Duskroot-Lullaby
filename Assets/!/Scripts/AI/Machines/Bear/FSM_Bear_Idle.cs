using UnityEngine;
using UnityEngine.AI;

public class FSM_Bear_Idle : State
{
    [SerializeField] float idleDuration = 5f;
    [SerializeField] State stateOnPlayerFound;
    [SerializeField] State stateOnEndTimer;

    Animator animator;
    VisionCollider visionCollider;
    NavMeshAgent navAgent;
    EnemySetup enemySetup;
    EnemyData enemyData;

    float timeSpent = 0f;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        animator = GetComponentInChildren<Animator>();
        stateMachine = GetComponent<StateMachine>();
        enemySetup = GetComponent<EnemySetup>();
    }
    public override void Enter()
    {
        if(navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();

        navAgent.isStopped = true;
        navAgent.updateRotation = false;
        navAgent.speed = 0f;
        timeSpent = 0f;
    }

    public override void Exit()
    {
        
    }

    public override void Process()
    {
        timeSpent += Time.deltaTime;
        if (timeSpent >= idleDuration)
        {
            stateMachine.ChangeState(stateOnEndTimer);
        }

        animator.SetFloat("Speed", navAgent.velocity.magnitude);
    }
}
