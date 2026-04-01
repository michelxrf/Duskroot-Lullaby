using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioDamageFly : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference damageFlyEvent;

    public void PlayDamageFly()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(damageFlyEvent);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }
}
