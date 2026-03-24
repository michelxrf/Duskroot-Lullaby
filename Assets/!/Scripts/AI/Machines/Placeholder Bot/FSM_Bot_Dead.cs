using UnityEngine;
using UnityEngine.AI;

public class FSM_Bot_Dead : State
{
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public override void Enter()
    {
        agent.isStopped = true;
    }

    public override void Exit()
    {
       
    }

    public override void Process()
    {
        
    }
}
