using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    public Button backButton;
    public Button exitButton;
    public Button settingsButton;
    public Button goToMenuButton;
    public GameObject settingsUI;

    private bool settingsOpen = false;

    void Start()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
            settingsOpen = false;
        }

        if (backButton != null) backButton.onClick.AddListener(ResumeGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (goToMenuButton != null) goToMenuButton.onClick.AddListener(exitToMenu);
    }

    void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void OpenSettings()
    {
        Debug.Log("Settings would be opened.");

        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
            settingsOpen = true;
        }

        exitButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        goToMenuButton.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        if (settingsUI != null && settingsOpen)
        {
            settingsUI.SetActive(false);
            settingsOpen = false;
            exitButton.gameObject.SetActive(true);
            settingsButton.gameObject.SetActive(true);
            goToMenuButton.gameObject.SetActive(true);
        }
        else
        {
            var pauseManager = Object.FindFirstObjectByType<PauseMenu>();
            pauseManager.ResumeGame();
        }
    }

    void exitToMenu()
    {
        SceneManager.LoadScene("Start Menu");

    }
}

