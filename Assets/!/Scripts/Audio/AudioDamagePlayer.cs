using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioDamagePlayer : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference damageEvent;

    public void PlayDamage()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(damageEvent);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }
}
