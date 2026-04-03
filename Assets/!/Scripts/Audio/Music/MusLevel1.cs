using FMOD.Studio;
using UnityEngine;
using FMODUnity;

public class MusLevel1 : MonoBehaviour
{
    public static MusLevel1 instance;

    [SerializeField] private EventReference musicEvent;
    private EventInstance musicInstance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    private void Start()
    {
        MusicManager.instance.StopMusic();
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.setParameterByName("Mus_Level1",0);
        musicInstance.start();
    }

    public void SetLevelMusicParameter(int value)
    {
        musicInstance.setParameterByName("Mus_Level1", value);
    }

    public void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }


}
