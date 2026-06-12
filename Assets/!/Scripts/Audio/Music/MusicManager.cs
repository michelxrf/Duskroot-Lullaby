using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private EventReference musicEvent;

    private EventInstance musicInstance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        musicInstance = RuntimeManager.CreateInstance(musicEvent);

    }

    public void StartMusic()
    {
        PLAYBACK_STATE playbackState;
        musicInstance.getPlaybackState(out playbackState);
        Debug.Log("MusMenu playing=" + PLAYBACK_STATE.PLAYING);
        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            musicInstance.start();
        }
    }

    public void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //musicInstance.release();
    }


}