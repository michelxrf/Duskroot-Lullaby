using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioBodyFallFly : MonoBehaviour
{

    [Header("FMOD")]
    [SerializeField] private EventReference bodyFallFlyEvent;

    public void PlayBodyFallFly()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(bodyFallFlyEvent);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }
}
