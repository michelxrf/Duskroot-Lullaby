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

    public override void Spawned()
    {
        characterController = GetComponent<SimpleKCC>();
    }

    public override void FixedUpdateNetwork()
    {
        if(!HasStateAuthority) return;

        if (GetInput(out NetworkInputData data))
        {
            if (data.Aim)
            {
                Debug.Log($"{gameObject.name} is aiming.");

                Vector2 mousePos = Mouse.current.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    Vector3 targetPoint = hitInfo.point;
                    TakeControl(this);
                    RotateTo(targetPoint - transform.position, this);
                }
            }
            else
            {
                ReleaseControl();
            }
        }
    }

    public void TakeControl(MonoBehaviour controller)
    {
        this.controller = controller;
    }

    public void ReleaseControl()
    {
        controller = null;
    }

    public void RotateTo(Vector3 direction, MonoBehaviour requester)
    {
        if (controller != null && requester != controller)
            return;

        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        characterController.SetLookRotation(new Vector2(0f, yaw));
    }

    public void AimAtTarget(Vector3 target)
    {
        if (!HasStateAuthority) return;

        TakeControl(this);
        // TODO: animation set
        RotateTo(target - transform.position, this);
        Debug.Log($"{gameObject.name} is aiming toward: {target}");
    }

    public void StopAiming()
    {
        if (!HasStateAuthority) return;

        ReleaseControl();
        // TODO: animation reset
        Debug.Log("Stopped aiming.");
    }
}
