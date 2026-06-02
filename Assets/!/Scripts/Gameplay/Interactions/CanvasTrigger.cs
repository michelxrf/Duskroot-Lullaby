using UnityEngine;

public class CanvasTrigger : MonoBehaviour
{
    [SerializeField] private GameObject targetCanvas;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponent<PlayerSetup>() == null)
            return;

        targetCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.GetComponent<PlayerSetup>() == null)
            return;

        targetCanvas.SetActive(false);
    }
}
