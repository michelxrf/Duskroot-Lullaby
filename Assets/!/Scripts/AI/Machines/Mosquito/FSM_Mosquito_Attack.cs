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
    VisionCollider visionCollider;

    Transform target;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        stateMachine = GetComponent<StateMachine>();

    }

    public override void Enter()
    {
        navAgent.isStopped = true;
        GetComponent<EnemyHealth>().OnHit += AttackInterrupted;

        target = visionCollider.GetClosestPlayer();

        // Rotate only around Y axis by keeping target at same height
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(targetPos);

        animator.SetTrigger("Attack");
    }

    public override void Exit()
    {
        GetComponent<EnemyHealth>().OnHit -= AttackInterrupted;
    }

    public override void Process()
    {
        
    }

    void ImpactFrame()
    {
        CombatFuncs.CastHitBox(hitboxCenter, gameObject, weaponData);
        stateMachine.ChangeState(stateAfterAttack);
    }

    void AttackInterrupted()
    {
        stateMachine.ChangeState(stateAfterAttack);
    }
}
