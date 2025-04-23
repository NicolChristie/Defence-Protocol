using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText; // Assign in inspector
    public TutorialLevelHandler levelHandler;
    public GameObject CoinPanel;
    public GameObject firstWeapon;
    public GameObject secondWeapon;
    public Button continueButton;
    public TutorialManager tutorialManager; // Reference to the TutorialManager


    private bool movementFinished = false;
    private bool WbuttonPressed = false;
    private bool AbuttonPressed = false;
    private bool SbuttonPressed = false;
    private bool DbuttonPressed = false;

    private void Start()
    {
        CoinPanel.SetActive(false);
        firstWeapon.SetActive(false);
        secondWeapon.SetActive(false);
        continueButton.gameObject.SetActive(false); // Hide at start
    }

    private void Update()
    {
        if (levelHandler == null) return;

        int levelIndex = levelHandler.GetCurrentLevelIndex();

        ShowTutorial(levelIndex);

        if (levelIndex == 0 && !movementFinished)
        {
            CheckMovementInput();
        }
    }

    void ShowTutorial(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0:
                tutorialText.text = "Welcome! Use the W A S D keys to move around.";
                break;
            case 1:
                tutorialText.text = "Weapons automatically shoot enemies in range.";
                firstWeapon.SetActive(true); // Show the first weapon
                break;
            case 2:
                tutorialText.text = "Stand on the weapon node to increase its effectiveness!";
                break;
            case 3:
                tutorialText.text = "💡 Remember: You can only carry one weapon at a time.";
                secondWeapon.SetActive(true); // Show the second weapon
                break;
            default:
                tutorialText.text = "";
                break;
        }
    }

    void CheckMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) WbuttonPressed = true;
        if (Input.GetKeyDown(KeyCode.A)) AbuttonPressed = true;
        if (Input.GetKeyDown(KeyCode.S)) SbuttonPressed = true;
        if (Input.GetKeyDown(KeyCode.D)) DbuttonPressed = true;

        if (WbuttonPressed && AbuttonPressed && SbuttonPressed && DbuttonPressed)
        {
            movementFinished = true;
            Debug.Log("✅ Movement tutorial complete!");
            TriggerNextStep(); // Trigger the next step in the tutorial
        }
    }

    public void TriggerNextStep(){
        continueButton.gameObject.SetActive(true); // Show the continue button
    }

    // Handle the continue button press to advance the tutorial
    public void NextLevel()
    {
        int levelIndex = levelHandler.GetCurrentLevelIndex();
        if (levelIndex == 1) // Transition to Level 2
        {
            levelHandler.ProceedToNextLevel(); // Proceed to the next level in the Level Handler
            tutorialText.text = "🔔 The tutorial is complete! Continue to the next level!";
        }
        else
        {
            levelHandler.ProceedToNextLevel(); // Proceed to the next level
        }

        // Hide the continue button after advancing to the next level
        continueButton.gameObject.SetActive(false);
    }
}

