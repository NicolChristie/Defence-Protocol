using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText; // Assign in inspector
    public GameObject tutorialPanel; // Assign in inspector
    public TutorialLevelHandler TutorialLevelHandler;
    public GameObject CoinPanel;
    public Button continueButton;
    public TutorialManager tutorialManager; // Reference to the TutorialManager
    public WeaponNode firstWeaponNode;
    public WeaponNode secondWeaponNode;
    public GameObject firstWeaponPrefab;
    public GameObject secondWeaponPrefab;
    public GameObject nextLevelButton;
    public GameObject healthBar;
    public GameObject healthtxt;
    public GameObject returnToShop;

    public GameObject goToMainMenuButton;
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
        healthBar.SetActive(false);
        healthtxt.SetActive(false);
        goToMainMenuButton.SetActive(false);
        RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
        panelRect.anchoredPosition = new Vector2(0f, 0f);
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
    RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
    switch (levelIndex)
    {
        case 0:
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
            tutorialText.text = "You can pick up and move weapons by pressing the 'E' key. The weapon doesnt shoot when it is being held.";
            {
                panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, -600f);
            }
            break;

        case 4:
            tutorialText.text = "You can also merge two identical weapons to create a more powerful one!";
            if (!hasPlacedSecondWeapon)
            {
                panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, 0f);
                PlaceWeaponOnNode(secondWeaponNode, secondWeaponPrefab);
                hasPlacedSecondWeapon = true;
            }
            break;
        case 5:
            tutorialText.text = "The health bar shows your ship's health. If it reaches 0, you lose the game."; 
            panelRect.anchoredPosition = new Vector2(116f, -100f);
            healthBar.SetActive(true);
            healthtxt.SetActive(true);
            ShipHealthBar.Instance.setHealthBar(100);
            break;
        case 6:
            tutorialText.text = "Coins are used to buy weapons.";
            CoinManager.Instance.setCoins(5);
            panelRect.anchoredPosition = new Vector2(-147f, -100f);
            CoinPanel.SetActive(true);
            returnToShop.SetActive(false);
            break;
        case 7:
            tutorialText.text = "Buy new weapons to make youself stronger, Good Luck!";
            CoinManager.Instance.setCoins(5);
            ShopManager.Instance.GenerateShop();
            ShopManager.Instance.ShowShop();
            nextLevelButton.SetActive(true);
            panelRect.anchoredPosition = new Vector2(0f, 0f);
            break;

        case 12:
            tutorialPanel.SetActive(true); // Hide the tutorial text
            tutorialText.text = "Well done! You have completed the tutorial!";
            goToMainMenuButton.SetActive(true);
            break;
        
        default:
            tutorialPanel.SetActive(false); 
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
            TriggerNextStep(1); // Trigger the next step in the tutorial
        }
    }

    public void TriggerNextStep(int levelIndex){
        if(levelIndex <= 6){
        continueButton.gameObject.SetActive(true); // Show the continue button
        }
    }

    // Handle the continue button press to advance the tutorial
    public void NextLevel()
    {
         if (WeaponNode.playerWeapon != null)
            {
                Debug.Log("⚠️ You must place the weapon down before continuing.");
                // Optionally show a message on the UI to notify the player
                tutorialText.text = "Place the weapon back on a node before continuing.";
                return;
            }
        int levelIndex = TutorialLevelHandler.GetCurrentLevelIndex();
        TutorialLevelHandler.ProceedToNextLevel(); // Proceed to the next level
        continueButton.gameObject.SetActive(false);
    }

    private void PlaceWeaponOnNode(WeaponNode node, GameObject weaponPrefab)
    {
        GameObject weapon = Instantiate(weaponPrefab, node.transform.position, Quaternion.identity, node.transform);
        Weaponprefab weaponComponent = weapon.GetComponent<Weaponprefab>();

        node.storedWeapon = weapon;
        node.storedWeaponPrefab = weaponComponent;
        weapon.transform.localScale = Vector3.one;

        // ✅ Assign original prefab so merge comparisons work correctly
        if (weaponComponent != null)
        {
            weaponComponent.originalPrefab = weaponPrefab;
        }
        else
        {
            Debug.LogWarning("Weaponprefab component missing from instantiated weapon.");
        }
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }
}

