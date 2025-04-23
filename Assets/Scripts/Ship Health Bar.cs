using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class ShipHealthBar : MonoBehaviour
{
    public GameObject healthBar; // Assign the Health Bar GameObject in the Inspector
    public int maxHP = 2;
    private int currentHP;
    private Vector3 fullScale = new Vector3(13f, 0.2f, 1f);

    // Game Over UI elements
    public GameObject gameOverCanvas;  // The Canvas that holds the Game Over screen UI
    public GameObject restartButton;  // The restart button
    public GameObject exitButton;  // The exit button

    void Start()
    {
        currentHP = maxHP;
        if (healthBar != null)
        {
            healthBar.transform.localScale = fullScale;
        }
        else
        {
            Debug.LogError("ShipHealthBar: healthBar GameObject is not assigned!");
        }

        // Initially, hide the Game Over canvas and buttons
        gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        // Example of taking damage (you can replace this with your own damage logic)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }

        // Check for game over when health reaches 0
        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHealthBar();
        Debug.Log("Ship took " + damage + " damage. Current HP: " + currentHP);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float healthPercent = Mathf.Clamp01((float)currentHP / maxHP);
            healthBar.transform.localScale = new Vector3(fullScale.x * healthPercent, fullScale.y, fullScale.z);
        }
    }

    private void GameOver()
    {
        // Pause the game
        Time.timeScale = 0;

        // Show the Game Over UI
        gameOverCanvas.SetActive(true);
    }

    public void ExitGame()
    {
        // Quit the game (this will only work in a built version)
        Debug.Log("Exiting game...");
        Application.Quit();
                // If in the editor:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void RestartGame()
    {
        // Restart the current scene (resets the game)
        Debug.Log("Restarting game...");
        Time.timeScale = 1;  // Ensure time is resumed
        SceneManager.LoadScene("Start Menu");
    }
}
