using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseSettings : MonoBehaviour
{
    public Button backButton;
    public Button exitButton;
    public Button settingsButton;
    public GameObject settingsUI;  // Reference to the settings menu UI (make sure it's not active at start)

    void Start()
    {
        // Disable settings UI at the start
        if (settingsUI != null)
            settingsUI.SetActive(false);

        if (backButton != null) backButton.onClick.AddListener(ResumeGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
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
        // Only open the settings when the button is pressed
        if (settingsUI != null)
            settingsUI.SetActive(true);
    }

    public void ResumeGame()
    {
        var pauseManager = Object.FindFirstObjectByType<PauseMenu>();
        pauseManager.ResumeGame();
    }
}

