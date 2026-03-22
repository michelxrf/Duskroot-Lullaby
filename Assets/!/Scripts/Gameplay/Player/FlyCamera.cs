using UnityEngine;

/// <summary>
/// A camera controller that follows a target object at a fixed offset.
/// Useful for creating third-person camera perspectives.
/// </summary>
public class FlyCamera : MonoBehaviour
{
    /// <summary>The target object for the camera to follow</summary>
    [HideInInspector] public Transform target;

    /// <summary>The position offset from the target</summary>
    [SerializeField] Vector3 offset;

    /// <summary>
    /// Updates camera position to follow the target at the specified offset.
    /// </summary>
    private void LateUpdate()
    {
        if(target == null) return;

        transform.position = target.position + offset;
    }
}
