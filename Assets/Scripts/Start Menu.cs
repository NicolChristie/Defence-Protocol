using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Button startButton;
    public Button tutorialButton;

    private string mainSceneName = "Main Scene"; // Replace with your actual main scene name
    //  [SerializeField] private string tutorialSceneName = "Tutorial";

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(ShowTutorial);
    }

    void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene(mainSceneName); // loads your main game scene
    }

    void ShowTutorial()
    {
        //SceneManager.LoadScene("tutorial");
    }
}
