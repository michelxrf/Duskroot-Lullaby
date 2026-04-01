using FMOD.Studio;
using UnityEngine;
using FMODUnity;

public class MusLevel1 : MonoBehaviour
{
    [SerializeField] private EventReference musicEvent;

    private EventInstance musicInstance;

    private void Start()
    {
        MusicManager.instance.StopMusic();
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

}
