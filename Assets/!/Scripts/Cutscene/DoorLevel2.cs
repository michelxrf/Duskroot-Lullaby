using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class DoorLevel2 : MonoBehaviour
{
    [SerializeField] GameObject cameraDoor;
    [SerializeField] Animator animationDoor;
    [SerializeField] float cameraTime=2.5f;

    private bool open = false;

    private void OnTriggerEnter(Collider other)
    {
        if (open) return;
        PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        if (otherPlayer == null) return;
        if (!otherPlayer.IsLocalPlayer()) return;
        open = true;
        if (cameraDoor != null) cameraDoor.SetActive(true);
        Invoke("OpenDoor", 1.5f);
    }

    private void DisableCamera()
    {
        if (cameraDoor != null) cameraDoor.SetActive(false);
    }

    private void OpenDoor()
    {
        animationDoor.SetTrigger("open");
        Invoke("DisableCamera", cameraTime);
    }

}
