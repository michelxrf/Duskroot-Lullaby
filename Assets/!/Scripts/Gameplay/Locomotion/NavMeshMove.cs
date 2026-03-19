using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Moves a character using Unity's NavMesh system.
/// </summary>
public class NavMeshMove : MonoBehaviour
{
    [SerializeField] Transform target;

    public void MoveTo(Transform destination)
    {
        target = destination;
        GetComponent<NavMeshAgent>().SetDestination(target.position);
    }

    public void Stop()
    {
        target = null;
        GetComponent<NavMeshAgent>().ResetPath();
    }
}
