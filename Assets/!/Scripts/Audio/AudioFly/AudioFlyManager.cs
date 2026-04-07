using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioFlyManager : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference idleFlyEvent;
    [SerializeField] private EventReference damageFlyEvent;
    [SerializeField] private EventReference fallFlyEvent;
    [SerializeField] private EventReference dieFlyEvent;

    private EventInstance idleInstance;

    private void Start()
    {
        PlayIdleFly();
    }
    /*
    private void Update()
    {
        if (idleInstance.isValid())
        {
            idleInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        }
    }
    */

    public void PlayIdleFly()
    {
        if (idleInstance.isValid()) return;

        idleInstance = RuntimeManager.CreateInstance(idleFlyEvent);
        idleInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        idleInstance.start();
    }

    public void StopIdleFly()
    {
        if (!idleInstance.isValid()) return;

        idleInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        idleInstance.release();
    }

    public void PlayDeathFly()
    {
        StopIdleFly(); 

        EventInstance instance = RuntimeManager.CreateInstance(dieFlyEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        instance.start();
        instance.release();
    }

    public void PlayDamageFly()
    {
        EventInstance instance = RuntimeManager.CreateInstance(damageFlyEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        instance.start();
        instance.release();
    }
    public void PlayBodyFallFly()
    {
        EventInstance instance = RuntimeManager.CreateInstance(fallFlyEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        instance.start();
        instance.release();
    }

    private void OnDestroy()
    {
        StopIdleFly();
    }
}