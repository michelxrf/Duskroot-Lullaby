using UnityEngine;

public class FollowPlayerPosition : MonoBehaviour
{
    private Transform playerTarget;
    private Camera mainCamera;

    [Header("Offset Gameplay")]
    public Vector3 offset = new Vector3(0, 1.6f, 0);

    private bool isInCutscene = false;

    void LateUpdate()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (isInCutscene)
        {
            FollowCamera();
        }
        else
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        if (playerTarget == null)
        {
            FindLocalPlayer();
            return;
        }

        transform.position = playerTarget.position + offset;

        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    void FollowCamera()
    {
        if (mainCamera == null) return;

        transform.position = mainCamera.transform.position;
        transform.rotation = mainCamera.transform.rotation;
    }

    void FindLocalPlayer()
    {
        PlayerSetup[] players = FindObjectsByType<PlayerSetup>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            if (player.IsLocalPlayer())
            {
                playerTarget = player.transform;
                break;
            }
        }
    }

    public void EnterCutscene()
    {
        isInCutscene = true;
    }

    public void ExitCutscene()
    {
        isInCutscene = false;
    }
}