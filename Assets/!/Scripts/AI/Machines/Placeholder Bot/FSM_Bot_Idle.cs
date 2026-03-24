using System;
using UnityEngine;
using UnityEngine.AI;

public class FSM_Bot_Idle : State
{
    Animator animator;
    NavMeshAgent navAgent;
    VisionCollider vision;

    [SerializeField] FSM_Bot_Follow OnSeePlayerState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<StateMachine>();
        navAgent = GetComponent<NavMeshAgent>();
        vision = GetComponentInChildren<VisionCollider>();
    }

    public override void Enter()
    {
        navAgent.isStopped = true;
        vision.OnPlayerEntered += PlayerOnVisual;
    }
    public override void Exit()
    {

    }
    public override void Process() { }

    void PlayerOnVisual(Transform playerTransform)
    {
        vision.OnPlayerEntered -= PlayerOnVisual;
        OnSeePlayerState.SetTarget(playerTransform);
        stateMachine.ChangeState(OnSeePlayerState);
    }

    private void OnDestroy()
    {
        vision.OnPlayerEntered -= PlayerOnVisual;
    }
}