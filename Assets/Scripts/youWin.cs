using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class youWin : MonoBehaviour
{
    public GameObject YouWinCanvas;
    public Button restartButton;
    public Button exitButton;
    public Button mainMenuButton;

    public RectTransform panelToAnimate;
    public Vector2 startPos = new Vector2(0, -800);
    public Vector2 endPos = new Vector2(0, 0);
    public float animationDuration = 1f;
    void Start()
    {
        Time.timeScale = 1f;
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        Debug.Log("YouWin: Start method called. Buttons initialized.");

        if (panelToAnimate != null)
        {
            panelToAnimate.anchoredPosition = startPos;
            StartCoroutine(AnimatePanel());
        }
    }
        IEnumerator AnimatePanel()
        {
            float elapsed = 0f;
            while (elapsed < animationDuration)
            {
                elapsed += Time.unscaledDeltaTime; // unscaled so it works even if Time.timeScale = 0
                float t = elapsed / animationDuration;
                panelToAnimate.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }
            panelToAnimate.anchoredPosition = endPos;
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
