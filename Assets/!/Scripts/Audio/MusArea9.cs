using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusArea9 : MonoBehaviour
{
    [SerializeField] private EventReference musicEvent;

    private EventInstance musicInstance;
    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        //PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        //if (otherPlayer != null) return;

        if (other.CompareTag("Player") && !isPlayerInside)
        {
            isPlayerInside = true;

            musicInstance = RuntimeManager.CreateInstance(musicEvent);
            musicInstance.start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerInside)
        {
            isPlayerInside = false;

            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
    }
}