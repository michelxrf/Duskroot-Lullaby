using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class FSM_Bot_Follow : State
{
    Transform target;
    NavMeshAgent agent;
    Animator animator;
    VisionCollider vision;

    [SerializeField] State OnLoseTarget;
    [SerializeField] float timeToForgetTarget = 5f; 

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<StateMachine>();
        vision = GetComponentInChildren<VisionCollider>();
    }

    public override void Enter()
    {
        vision.OnPlayerLeft += LostPlayer;
        vision.OnPlayerEntered += StartTimer;
    }

    void StartTimer(Transform ignore = null)
    {
        StopAllCoroutines();
        StartCoroutine(CountDown(timeToForgetTarget));
    }

    public override void Exit()
    {
        target = null;
        vision.OnPlayerLeft -= LostPlayer;
        vision.OnPlayerEntered -= (ctx) => { StopAllCoroutines(); };
        animator.SetFloat("Speed", 0f);
    }

    public override void Process()
    {
        agent.SetDestination(target.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void SetTarget(Transform transformToFollow)
    {
        agent.isStopped = false;
        target = transformToFollow;
    }

    void LostPlayer()
    {
        StartTimer();
    }

    private void OnDestroy()
    {
        vision.OnPlayerLeft -= LostPlayer;
    }

    IEnumerator CountDown(float time)
    {
        yield return new WaitForSeconds(time);
        stateMachine.ChangeState(OnLoseTarget);
    }
}
