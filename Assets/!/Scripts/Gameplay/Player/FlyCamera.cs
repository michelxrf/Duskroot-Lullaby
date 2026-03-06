using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [SerializeField] Vector3 offset;

    private void LateUpdate()
    {
        if(target == null) return;

        transform.position = target.position + offset;
    }
}
