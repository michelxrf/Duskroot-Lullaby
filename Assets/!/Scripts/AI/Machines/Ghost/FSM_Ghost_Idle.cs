using UnityEngine;
using UnityEngine.AI;

public class FSM_Ghost_Idle : State
{
    [SerializeField] float idleDuration = 5f;
    [SerializeField] State stateOnPlayerFound;
    [SerializeField] State stateOnEndTimer;

    VisionCollider visionCollider;
    NavMeshAgent navAgent;
    EnemySetup enemySetup;
    EnemyData enemyData;

    float timeSpent = 0f;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        stateMachine = GetComponent<StateMachine>();
        enemySetup = GetComponent<EnemySetup>();
    }
    public override void Enter()
    {
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
    }
}
