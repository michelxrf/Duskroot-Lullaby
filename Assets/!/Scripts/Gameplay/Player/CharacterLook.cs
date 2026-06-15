using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine.InputSystem;
using CombatSystem;

/// <summary>
/// Allow the player character to look/aim toward the mouse position
/// </summary>
public class CharacterLook : NetworkBehaviour
{
    [SerializeField] GameObject reticlePrefab;
    [SerializeField] float aimConeAngle = 90f;
    [SerializeField] float aimRange = 7f;
    [SerializeField] float attackRotationDuration = 0.5f;
    [SerializeField] float reticleVerticalOffset = .5f;

    SimpleKCC characterController;
    MonoBehaviour controller;
    Animator animator;
    Vector3 lookingAt;

    public Transform lockedTarget;
    GameObject reticleInstance;

    [Networked] int attackEndTick { get; set; }

    public override void Spawned()
{
        characterController = GetComponent<SimpleKCC>();
        animator = GetComponent<Animator>();

        if (HasStateAuthority && reticlePrefab != null)
        {
            reticleInstance = Instantiate(reticlePrefab);
            reticleInstance.SetActive(false);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(!HasStateAuthority) return;

        if (GetInput(out NetworkInputData data))
        {
            Vector3 mouseGroundPos = lookingAt;
            bool groundHit = false;

            if (data.Aim || data.Look != Vector2.zero)
            {
                if (InputDeviceManager.Instance.CurrentDevice == InputDeviceType.KeyboardMouse)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                

                    Ray ray = Camera.main.ScreenPointToRay(mousePos);
                    LayerMask layerMask = LayerMask.GetMask("Ground");
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        mouseGroundPos = hitInfo.point;
                        groundHit = true;
                    }
                }
                else
                {
                    mouseGroundPos = new Vector3(data.Look.x, 0f, data.Look.y) * aimRange + transform.position;
                    groundHit = true;
                }

                TakeControl(this);
                FindBestTarget();

                if (lockedTarget != null)
                {
                    lookingAt = lockedTarget.position;
                }
                else if (groundHit)
                {
                    lookingAt = mouseGroundPos;
                }

                if (reticleInstance != null)
                {
                    reticleInstance.SetActive(true);
                    reticleInstance.transform.position = lookingAt + Vector3.up * reticleVerticalOffset;
                }

                RotateTo(lookingAt - transform.position, this);
            }
            else
            {
                ReleaseControl();
                lockedTarget = null;
                if (reticleInstance != null) reticleInstance.SetActive(false);
            }
        }
    }

    void FindBestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, aimRange);
        float closestDist = float.MaxValue;
        Transform bestTarget = null;

        foreach (var col in colliders)
        {
            // Check for EnemySetup or Breakable
            var enemy = col.GetComponent<EnemySetup>();
            var breakable = col.GetComponent<Breakable>();

            if (enemy != null || breakable != null)
            {
                Transform target = enemy != null ? enemy.transform : breakable.transform;

                // Ignore dead targets
                var health = target.GetComponent<Health>();
                if (health != null && health.IsDead()) continue;

                Vector3 dirToTarget = (target.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);

                if (angle <= aimConeAngle * 0.5f)
                {
                    float dist = Vector3.Distance(transform.position, target.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        bestTarget = target;
                    }
                }
            }
        }

        lockedTarget = bestTarget;
    }

    public void OnAttackTriggered()
    {
        if (Object.HasStateAuthority)
        {
            attackEndTick = Runner.Tick + (int)(attackRotationDuration / Runner.DeltaTime);
        }
    }

    public bool IsAiming()
{
        return controller != null;
    }

    public Vector3 GetLookingTarget()
    {
        return lookingAt;
    }

    void TakeControl(MonoBehaviour controller)
    {
        animator.SetBool("IsAiming", true);
        this.controller = controller;
    }

    void ReleaseControl()
    {
        animator.SetBool("IsAiming", false);
        controller = null;
    }

    public void RotateTo(Vector3 direction, MonoBehaviour requester)
    {
        if (controller != null && requester != controller)
            return;

        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        characterController.SetLookRotation(new Vector2(0f, yaw));
    }
}
