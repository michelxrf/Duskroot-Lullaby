using CombatSystem;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class FSM_Mosquito_Attack : State
{
    [SerializeField] Transform hitboxCenter;
    [SerializeField] State stateAfterAttack;
    [SerializeField] WeaponData weaponData;


    Animator animator;
    NavMeshAgent navAgent;

    Transform target;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        stateMachine = GetComponent<StateMachine>();

    }

    public override void Enter()
    {
        navAgent.isStopped = true;

        
        animator.SetTrigger("Attack");
    }

    public override void Exit()
    {
        
    }

    public override void Process()
    {
        
    }

    void ImpactFrame()
    {
        CombatFuncs.CastHitBox(hitboxCenter, gameObject, weaponData);
        stateMachine.ChangeState(stateAfterAttack);
    }
}
