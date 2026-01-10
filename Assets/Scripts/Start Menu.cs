using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio; // Needed for AudioMixer

public class StartMenu : MonoBehaviour
{
    public Button startButton;
    public Button tutorialButton;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer; // Drag your AudioMixer here

    private string mainSceneName = "Main Scene"; 
    private string tutorialSceneName = "Tutorial";

    // Same keys used in SoundMixerManager
    private const string MasterKey = "MasterVolume";
    private const string SfxKey = "SfxVolume";
    private const string MusicKey = "MusicVolume";

    void Start()
    {
        Time.timeScale = 1f;

        // Apply saved audio levels at game start
        ApplySavedAudioSettings();

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(ShowTutorial);
    }

    private void ApplySavedAudioSettings()
    {
        float masterValue = PlayerPrefs.GetFloat(MasterKey, 0f); // Default 0 dB
        float sfxValue    = PlayerPrefs.GetFloat(SfxKey, 0f);
        float musicValue  = PlayerPrefs.GetFloat(MusicKey, 0f);

        audioMixer.SetFloat("masterVolume", masterValue);
        audioMixer.SetFloat("soundFXVolume", sfxValue);
        audioMixer.SetFloat("musicVolume", musicValue);

        Debug.Log($"Loaded audio settings: Master={masterValue}, SFX={sfxValue}, Music={musicValue}");
    }

    void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene(mainSceneName); 
    }

    void ShowTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }
}
