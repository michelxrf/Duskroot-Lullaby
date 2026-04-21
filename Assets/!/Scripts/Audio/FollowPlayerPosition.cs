using UnityEngine;

public class FollowPlayerPosition : MonoBehaviour
{
    private Transform target;
    private bool rotationInitialized = false;

    [Header("Offset do Listener")]
    public Vector3 offset = new Vector3(0, 1.6f, 0);

    void Update()
    {
        if (target == null)
        {
            FindLocalPlayer();
            return;
        }

        transform.position = target.position + offset;

        if (!rotationInitialized)
        {
            transform.rotation = target.rotation;
            rotationInitialized = true;
        }
    }

    void FindLocalPlayer()
    {
        PlayerSetup[] players = FindObjectsByType<PlayerSetup>(FindObjectsSortMode.None);
        Debug.Log("Procurando player");
        foreach (var player in players)
        {
            if (player.IsLocalPlayer())
            {
                target = player.transform;
                break;
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        transform.rotation = newTarget.rotation;
        rotationInitialized = true;
    }
}