using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsUI;

    public Button backButton;
    public Button exitButton;
    public Button settingsButton;

    private bool isPaused = false;

    void Start()
    {
        // Hook up the button events
        backButton.onClick.AddListener(ResumeGame);
        exitButton.onClick.AddListener(ExitGame);
        settingsButton.onClick.AddListener(OpenSettings);

        // Ensure menu is hidden on start
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // This pauses all movement/physics
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void ExitGame()
    {
        Debug.Log("Exiting Game...");
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        Application.Quit();

        // If in the editor:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void OpenSettings()
    {
        Debug.Log("Settings would be opened.");
        settingsUI.SetActive(true);
    }
}
