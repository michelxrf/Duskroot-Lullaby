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
    }
    public override void Exit()
    {

    }
    public override void Process() { }

    void PlayerOnVisual(Transform playerTransform)
    {

        OnSeePlayerState.SetTarget(playerTransform);
        stateMachine.ChangeState(OnSeePlayerState);
    }

    private void OnDestroy()
    {
    }
}