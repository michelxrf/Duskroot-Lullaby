using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioDeathFly : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference deathFlyEvent;

    public void PlayDeathFly()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(deathFlyEvent);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }
}
