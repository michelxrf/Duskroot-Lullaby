using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioBodyFallPlayer : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference bodyFallPlayerEvent;

    public void PlayBodyFallPlayer()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(bodyFallPlayerEvent);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }
}
