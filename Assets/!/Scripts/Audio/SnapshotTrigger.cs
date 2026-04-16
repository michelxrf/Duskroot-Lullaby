using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SnapshotTrigger : MonoBehaviour
{
    [Header("Snapshots")]
    [SerializeField] private EventReference interiorSnapshot;
    [SerializeField] private EventReference exteriorSnapshot;

    private EventInstance interiorInstance;
    private EventInstance exteriorInstance;

    private void Start()
    {
        interiorInstance = RuntimeManager.CreateInstance(interiorSnapshot);
        exteriorInstance = RuntimeManager.CreateInstance(exteriorSnapshot);
        exteriorInstance.start();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        if (otherPlayer == null) return;

        if (otherPlayer.IsLocalPlayer())
        {
            interiorInstance.start();
            exteriorInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        if (otherPlayer == null) return;
        if (otherPlayer.IsLocalPlayer())
        {
            interiorInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            exteriorInstance.start();
        }
    }

    private void OnDestroy()
    {
        interiorInstance.release();
        exteriorInstance.release();
    }
}