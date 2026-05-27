using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioDeathPlayer : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference deathEvent;

    private PlayerSetup playerSetup;
    CharacterGender gender;

    private void Start()
    {
        gender = playerSetup.GetGender();
    }
    public void PlayDeath()
    {
        gender = playerSetup.GetGender();
        EventInstance deathInstance = RuntimeManager.CreateInstance(deathEvent);
        deathInstance.setParameterByName("Gen", (float)gender);
        deathInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        deathInstance.start();
        deathInstance.release();
    }
    }
