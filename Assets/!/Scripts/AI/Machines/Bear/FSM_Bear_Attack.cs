using CombatSystem;
using UnityEngine;
using UnityEngine.AI;

public class FSM_Bear_Attack : State
{
    [SerializeField] Transform hitboxCenter;
    [SerializeField] State stateAfterAttack;
    [SerializeField] WeaponData weaponData;


    Animator animator;
    NavMeshAgent navAgent;
    VisionCollider visionCollider;
    WeaponDataInstance weaponDataInstance;

    Transform target;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        visionCollider = GetComponentInChildren<VisionCollider>();
        stateMachine = GetComponent<StateMachine>();

        weaponDataInstance = new WeaponDataInstance(weaponData, 0, "1");
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
        animator.SetFloat("Speed", navAgent.velocity.magnitude);
    }

    void ImpactFrame()
    {
        CombatFuncs.CastHitBox(hitboxCenter, gameObject, weaponDataInstance);
        stateMachine.ChangeState(stateAfterAttack);
    }

    void AttackInterrupted()
    {
        stateMachine.ChangeState(stateAfterAttack);
    }
}
