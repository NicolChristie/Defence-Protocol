using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText; // Assign in inspector
    public TutorialLevelHandler TutorialLevelHandler;
    public GameObject CoinPanel;
    public Button continueButton;
    public TutorialManager tutorialManager; // Reference to the TutorialManager
    public WeaponNode firstWeaponNode;
    public WeaponNode secondWeaponNode;
    public GameObject firstWeaponPrefab;
    public GameObject secondWeaponPrefab;
    private bool hasPlacedFirstWeapon = false;
    private bool hasPlacedSecondWeapon = false;
    private int lastTutorialIndex = -1;



    private bool movementFinished = false;
    private bool WbuttonPressed = false;
    private bool AbuttonPressed = false;
    private bool SbuttonPressed = false;
    private bool DbuttonPressed = false;

    private void Start()
    {
        CoinPanel.SetActive(false);
        continueButton.gameObject.SetActive(false); // Hide at start
    }

    private void Update()
    {
        if (TutorialLevelHandler == null) return;

        int levelIndex =  TutorialLevelHandler.GetCurrentLevelIndex();

        ShowTutorial(levelIndex);

        if (levelIndex == 0 && !movementFinished)
        {
            CheckMovementInput();
        }
    }

    void ShowTutorial(int levelIndex)
{
    if (lastTutorialIndex == levelIndex) return; // only update once per level change
    lastTutorialIndex = levelIndex;

    switch (levelIndex)
    {
        case 0:
        PlaceWeaponOnNode(secondWeaponNode, secondWeaponPrefab);
        PlaceWeaponOnNode(firstWeaponNode, firstWeaponPrefab);
            tutorialText.text = "Welcome! Use the W A S D keys to move around.";
            break;

        case 1:
            tutorialText.text = "Weapons automatically shoot enemies in range.";
            if (!hasPlacedFirstWeapon)
            {
                PlaceWeaponOnNode(firstWeaponNode, firstWeaponPrefab);
                hasPlacedFirstWeapon = true;
            }
            break;

        case 2:
            tutorialText.text = "Stand on the weapon node to increase its effectiveness!";
            break;

        case 3:
            tutorialText.text = "You can pick up and move weapons by pressing the 'E' key.";
            tutorialText.rectTransform.anchoredPosition = new Vector2(tutorialText.rectTransform.anchoredPosition.x, -550f);
            break;

        case 4:
            tutorialText.text = "You can also merge two identical weapons to create a more powerful one!";
            if (!hasPlacedSecondWeapon)
            {
                PlaceWeaponOnNode(secondWeaponNode, secondWeaponPrefab);
                hasPlacedSecondWeapon = true;
            }
            tutorialText.rectTransform.anchoredPosition = new Vector2(tutorialText.rectTransform.anchoredPosition.x, 0f);
            break;

        case 5:
            tutorialText.text = "Buy different weapons from the shop to increase your firepower!";
            CoinManager.Instance.AddCoins(5);
            ShopManager.Instance.GenerateShop();
            TutorialLevelHandler.nextLevelButton.SetActive(true);
            ShopManager.Instance.ShowShop();
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
        int levelIndex = TutorialLevelHandler.GetCurrentLevelIndex();
        if (levelIndex == 1) // Transition to Level 2
        {
            TutorialLevelHandler.ProceedToNextLevel(); // Proceed to the next level in the Level Handler
            tutorialText.text = "🔔 The tutorial is complete! Continue to the next level!";
        }
        else
        {
            TutorialLevelHandler.ProceedToNextLevel(); // Proceed to the next level
        }

        // Hide the continue button after advancing to the next level
        continueButton.gameObject.SetActive(false);
    }

    private void PlaceWeaponOnNode(WeaponNode node, GameObject weaponPrefab)
{
    GameObject weapon = Instantiate(weaponPrefab, node.transform.position, Quaternion.identity, node.transform);
    node.storedWeapon = weapon;
    node.storedWeaponPrefab = weapon.GetComponent<Weaponprefab>();
    weapon.transform.localScale = Vector3.one;
}

}

