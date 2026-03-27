using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class AudioPunchPlayer : MonoBehaviour
{
    public enum Gender { Woman, Man, Girl, Boy }

    [Header("Person")]
    public Gender gender;

    [Header("FMOD")]
    [SerializeField] private EventReference punchEvent;

    public void PlayPunch()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(punchEvent);
        punchInstance.setParameterByName("Gen", (float)gender);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }

}
