using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class PunchPlayer : MonoBehaviour
{
    public enum Gender { Female, Male }

    [Header("Person")]
    public Gender gender;

    [Header("FMOD")]
    [SerializeField] private EventReference punchEvent;

    public void PlayPlayPunch()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(punchEvent);
        punchInstance.setParameterByName("Gen", (float)gender);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }

}
