using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    private bool isPaused = false;
    private string pauseMenu = "PauseSettings";

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
                Debug.Log("Escape key pressed");
            if (isPaused){
                Debug.Log("Resuming game");
                ResumeGame();
            }else{
                Debug.Log("Pausing game");
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        if (!isPaused)
        {
            SceneManager.LoadScene(pauseMenu, LoadSceneMode.Additive);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync(pauseMenu);
            isPaused = false;
        }
    }

}
