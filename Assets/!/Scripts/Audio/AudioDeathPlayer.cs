using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioDeathPlayer : MonoBehaviour
{
    public enum GenderDeath { Woman, Man, Girl, Boy }

    [Header("Person")]
    public GenderDeath gender;

    [Header("FMOD")]
    [SerializeField] private EventReference deathEvent;

    public void PlayDeath()
    {
        EventInstance deathInstance = RuntimeManager.CreateInstance(deathEvent);
        deathInstance.setParameterByName("Gen", (float)gender);
        deathInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        deathInstance.start();
        deathInstance.release();
    }
}
