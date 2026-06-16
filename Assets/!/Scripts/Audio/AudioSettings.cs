using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

public class AudioSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider mainSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("FMOD VCA Paths")]
    [SerializeField] private string mainVCAPath = "vca:/Main";
    [SerializeField] private string musicVCAPath = "vca:/Music";
    [SerializeField] private string sfxVCAPath = "vca:/SFX";

    private VCA musicVCA;
    private VCA sfxVCA;
    private VCA mainVCA;

    void Start()
    {
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);
        sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);
        mainVCA = RuntimeManager.GetVCA(mainVCAPath);

        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float savedSFXVol   = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        float savedMainVol  = PlayerPrefs.GetFloat("MainVolume", 0.75f);

        musicSlider.value = savedMusicVol;
        sfxSlider.value = savedSFXVol;
        mainSlider.value = savedMainVol;

        SetMusicVolume(savedMusicVol);
        SetSFXVolume(savedSFXVol);
        SetMainVolume(savedMainVol);
        musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(musicSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(sfxSlider.value); });
        mainSlider.onValueChanged.AddListener(delegate { SetMainVolume(mainSlider.value); });
    }

    public void SetMusicVolume(float volume)
    {
        musicVCA.setVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVCA.setVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetMainVolume(float volume)
    {
        mainVCA.setVolume(volume);
        PlayerPrefs.SetFloat("MainVolume", volume);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}