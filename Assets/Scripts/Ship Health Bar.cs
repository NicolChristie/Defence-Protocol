using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Don't forget this!

public class ShipHealthBar : MonoBehaviour
{
    public GameObject healthBar;
    public int maxHP = 2;
    private int currentHP;
    private Vector3 fullScale = new Vector3(13f, 0.2f, 1f);
    public static ShipHealthBar Instance;

    // Game Over UI elements
    public GameObject gameOverCanvas;
    public GameObject restartButton;
    public GameObject exitButton;

    // ➕ Add this: UI Text for showing HP
    public TextMeshProUGUI hpText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }  

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

        if (hpText != null)
        {
            UpdateHPText(); // Initialize the text
        }

        gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    public void setHealthBar(int health)
    {
        currentHP = health;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHealthBar();
        UpdateHPText(); // 🔄 Update the text whenever HP changes
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHealthBar();
        UpdateHPText(); // 🔄 Update the text whenever HP changes
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

    // 🆕 This method updates the text
    private void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = currentHP + " / " + maxHP;
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        gameOverCanvas.SetActive(true);
    }

    

    public void YouWin()
    {
        Time.timeScale = 0;
        SceneManager.LoadScene("YouWin");
    }


    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Scene");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Start Menu");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
