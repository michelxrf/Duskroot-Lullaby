using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class DamageAudioPlayer : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference damageEvent;

    public void PlayPlayDamage()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(damageEvent);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }
}
