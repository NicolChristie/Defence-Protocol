using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public Slider masterVolumeSlider;
    public Slider soundFXVolumeSlider;
    public Slider musicVolumeSlider;

    private const string MasterKey = "MasterVolume";
    private const string SfxKey = "SfxVolume";
    private const string MusicKey = "MusicVolume";

    public float defaultVolume = -10f;
    void Start()
    {
        Debug.Log("Initializing SoundMixerManager...");
        if (masterVolumeSlider == null) masterVolumeSlider = GameObject.Find("Master Slider")?.GetComponent<Slider>();
        if (soundFXVolumeSlider == null) soundFXVolumeSlider = GameObject.Find("SoundFX Slider")?.GetComponent<Slider>();
        if (musicVolumeSlider == null)    musicVolumeSlider = GameObject.Find("Music Slider")?.GetComponent<Slider>();

        // Restore saved values (default 0 dB if not saved yet)
        float masterValue = PlayerPrefs.GetFloat(MasterKey, defaultVolume);
        float sfxValue    = PlayerPrefs.GetFloat(SfxKey, defaultVolume);
        float musicValue  = PlayerPrefs.GetFloat(MusicKey, defaultVolume);

        masterVolumeSlider.value = masterValue;
        soundFXVolumeSlider.value = sfxValue;
        musicVolumeSlider.value = musicValue;

        setMasterVolume(masterValue);
        setSoundFXVolume(sfxValue);
        setMusicVolume(musicValue);

        // Add listeners
        masterVolumeSlider.onValueChanged.AddListener(setMasterVolume);
        soundFXVolumeSlider.onValueChanged.AddListener(setSoundFXVolume);
        musicVolumeSlider.onValueChanged.AddListener(setMusicVolume);
    }

    public void setMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", level);
        PlayerPrefs.SetFloat(MasterKey, level);
    }

    public void setSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", level);
        PlayerPrefs.SetFloat(SfxKey, level);
    }

    public void setMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", level);
        PlayerPrefs.SetFloat(MusicKey, level);
    }
}
