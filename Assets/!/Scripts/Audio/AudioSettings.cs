using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

public class AudioSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("FMOD VCA Paths")]
    [SerializeField] private string musicVCAPath = "vca:/Music";
    [SerializeField] private string sfxVCAPath = "vca:/SFX";

    private VCA musicVCA;
    private VCA sfxVCA;

    void Start()
    {
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);
        sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);

        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float savedSFXVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        musicSlider.value = savedMusicVol;
        sfxSlider.value = savedSFXVol;

        SetMusicVolume(savedMusicVol);
        SetSFXVolume(savedSFXVol);

        musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(musicSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(sfxSlider.value); });
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

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}