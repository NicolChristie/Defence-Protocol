using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Button startButton;
    public Button tutorialButton;
    private WeaponNode weaponNode; 

    private string mainSceneName = "Main Scene"; 
    private string tutorialSceneName = "Tutorial";

    void Start()
    {
        Time.timeScale = 1f;
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(ShowTutorial);

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
