using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class youWin : MonoBehaviour
{
    public GameObject YouWinCanvas;
    public Button restartButton;
    public Button exitButton;
    public Button mainMenuButton;

    void Start()
    {
        // Set up button listeners
        Time.timeScale = 1f;
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        Debug.Log("YouWin: Start method called. Buttons initialized.");

        // Get reference to WeaponNode
        
    }
    void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Scene");
    }

    void GoToMainMenu()
    {
        Debug.Log("Going to main menu...");
        Time.timeScale = 1;
        SceneManager.LoadScene("Start Menu");
    }
    void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
