using UnityEngine;

public class CanvasTrigger : MonoBehaviour
{
    [SerializeField] private GameObject targetCanvas;

    private void OnTriggerEnter(Collider collision)
    {
        PlayerSetup player = collision.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        targetCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider collision)
    {
        PlayerSetup player = collision.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        targetCanvas.SetActive(false);
    }
}
