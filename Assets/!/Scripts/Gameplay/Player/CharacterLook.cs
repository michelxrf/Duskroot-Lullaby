using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine.InputSystem;

/// <summary>
/// Allow the player character to look/aim toward the mouse position
/// </summary>
public class CharacterLook : NetworkBehaviour
{
    SimpleKCC characterController;
    MonoBehaviour controller;
    Animator animator;
    Vector3 lookingAt;
    PlayerMovement playerMovement;

    public override void Spawned()
    {
        characterController = GetComponent<SimpleKCC>();
        animator = GetComponent<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if(!HasStateAuthority) return;

        if (GetInput(out NetworkInputData data))
        {
            if (data.Aim)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                LayerMask layerMask = LayerMask.GetMask("Ground");
                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                {
                    lookingAt = hitInfo.point;
                    TakeControl(this);
                    RotateTo(lookingAt - transform.position, this);

                }
            }
            else
            {
                ReleaseControl();
            }
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
