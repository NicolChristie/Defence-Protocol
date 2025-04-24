using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TutorialLevelHandler : MonoBehaviour
{
    public EnemySpawner enemySpawner; // Reference to EnemySpawner
    public GameObject player; // Reference to the player
    public GameObject nextLevelButton; // Reference to the Next Level Button
    public EnemyType[] enemyTypes; // Reference to different enemy types
    private int levelIndex = 0;

    public TutorialManager tutorialManager; // Reference to the TutorialManager


    public TextAsset levelFile; // Drag your Levels.txt file here in the Inspector
    private bool isLevelComplete = false; // Flag to check if level is complete
    private bool isGameStarted = false; // Flag to track if the game has started

   

    // 🔄 Start the game when the user presses  "Start"
    public void StartLevel()
    {
        isGameStarted = true;

        nextLevelButton.SetActive(false);


        // Start the level with a delay
        StartCoroutine(HandleLevelWithDelay(1)); // Start Level 1 with a delay
    }

    // 🔄 Handle the level (spawns enemies) with a delay before the level starts
    public IEnumerator HandleLevelWithDelay(int levelIndex)
    {
        // Wait for 2 seconds before starting the level
        yield return new WaitForSeconds(0.5f);

        // Start the level (this starts spawning enemies)
        StartCoroutine(HandleLevel(levelIndex));
    }

    public IEnumerator HandleLevel(int levelIndex)
    {
        if (!isGameStarted) yield break; // If the game hasn't started, do nothing

        Debug.Log($"🚀 Loading Level {levelIndex}");

        // Load level data from file
        List<string> levelSetup = LoadLevelFromFile(levelFile, levelIndex);

        // If no data found for this level, stop
        if (levelSetup.Count == 0)
        {
            Debug.LogError($"❌ Level {levelIndex} is empty!");
            yield break;
        }

        // Spawn enemies for each wave in the level
        foreach (string wave in levelSetup)
        {
            SpawnEnemiesInWave(wave);
            yield return new WaitForSeconds(3f); // Delay between waves
        }

        // Wait until all level-specific enemies are destroyed (only 3 persistent ones remain)
        while (!AreOnlyPersistentEnemiesLeft)
        {
            yield return null; // Keep checking each frame
        }

        // Now check if the player is carrying a weapon
        if (WeaponNode.playerWeapon != null)
        {
            Debug.Log("Player is still carrying a weapon. Cannot finish level yet.");

            // Wait until the player places the weapon down before proceeding
            yield return StartCoroutine(WaitForWeaponToBePlacedDown());

            Debug.Log("Player has placed the weapon down. Proceeding with level completion.");
        }

        Debug.Log("🎉 Level Complete!");

        // Add a check to ensure that the level hasn't already been completed
        if (isLevelComplete) yield break; // Prevent duplicate execution

        isLevelComplete = true;
        if (levelIndex < 8){
            Debug.Log("showing tutorial manager and next level button");
            if (tutorialManager != null)
            {
                tutorialManager.TriggerNextStep(levelIndex);
            }
        }else
        {
            Debug.Log("showing shop and next level button");
            CoinManager.Instance.AddCoins(5); // Reward for finishing the level
            ShopManager.Instance.GenerateShop();
            nextLevelButton.SetActive(true); // Show the next level button
            ShopManager.Instance.ShowShop(); 
        }        
        Debug.Log($"Level {levelIndex} completed! Proceeding to next level.");
    }

    // Wait until the player places down the weapon
    private IEnumerator WaitForWeaponToBePlacedDown()
    {
        while (WeaponNode.playerWeapon != null) // While the player is holding a weapon
        {
            yield return null; // Keep checking every frame
        }

        // Once the player has placed the weapon down, proceed
        Debug.Log("Weapon has been placed down.");
    }

    
    public void ProceedToNextLevel()
    {
        levelIndex++; // Increment the level index
        isLevelComplete = false; // Reset level complete flag before starting the next level

        Debug.Log($"Next level button pressed! Current level: {levelIndex}");

        ShopManager.Instance.HideShop();

        List<string> nextLevelData = LoadLevelFromFile(levelFile, levelIndex);

        if (nextLevelData.Count > 0)
        {
            nextLevelButton.SetActive(false); // Hide the next level button
            StartCoroutine(StartLevelWithDelay(levelIndex));

        }
        else
        {
            Debug.Log("🎉 All levels complete!");
        }
    }

    // 🔄 Load a specific level from the file
    public List<string> LoadLevelFromFile(TextAsset levelTextAsset, int targetLevel)
{
    List<string> levelData = new List<string>();

    if (levelTextAsset == null)
    {
        Debug.LogError("❌ Level file is not assigned!");
        return levelData;
    }

    string[] lines = levelTextAsset.text.Split('\n');
    bool isReading = false;

    foreach (string line in lines)
    {
        if (string.IsNullOrWhiteSpace(line)) continue;

        if (line.StartsWith($"Level {targetLevel}:"))
        {
            isReading = true;
            Debug.Log($"Reading data for Level {targetLevel}...");
            continue;
        }

        if (line.StartsWith("Level ") && isReading)
        {
            Debug.Log($"Ending reading data for Level {targetLevel}");
            break;
        }

        if (isReading)
        {
            levelData.Add(line.Trim());
        }
    }

    if (levelData.Count == 0)
    {
        Debug.LogWarning($"⚠️ No data found for Level {targetLevel}");
    }

    return levelData;
}


    // 🔄 Spawn enemies in a wave based on the level data
    void SpawnEnemiesInWave(string wave)
    {
        string cleanedWave = wave.Replace("[", "").Replace("]", "").Trim(); // Clean up wave data
        string[] enemyGroups = cleanedWave.Split(','); // Split into individual enemy groups

        foreach (string group in enemyGroups)
        {
            string trimmedGroup = group.Trim(); // Remove spaces
            if (string.IsNullOrEmpty(trimmedGroup)) continue;

            int count = 0;
            string enemyLetter = "";
            string spawnDirection = "";

            // Parse the group into count, enemy type and direction
            foreach (char c in trimmedGroup)
            {
                if (char.IsDigit(c))
                    count = count * 10 + (c - '0'); // Convert to integer
                else if (char.IsLetter(c))
                    enemyLetter += c; // Get enemy identifier
            }

            // Extract the spawn direction (after the dot)
            if (trimmedGroup.Contains("."))
            {
                spawnDirection = trimmedGroup.Split('.')[1].ToUpper(); // Direction is after the dot (e.g., L, R, U, D)
            }

            // Validate the spawn direction
            if (spawnDirection != "L" && spawnDirection != "R" && spawnDirection != "U" && spawnDirection != "D")
            {
                Debug.LogError($"❌ Invalid spawn direction: {spawnDirection} in wave {trimmedGroup}");
                continue; // Skip this enemy group if direction is invalid
            }

            if (count > 0 && enemyLetter.Length > 0)
            {
                EnemyType enemyType = GetEnemyTypeByIdentifier(enemyLetter[0]);

                if (enemyType != null)
                {
                    Debug.Log($"🛡️ Spawning {count} enemies of type {enemyLetter} at {spawnDirection}");
                    for (int i = 0; i < count; i++)
                    {
                        SpawnEnemy(enemyType, spawnDirection);
                    }
                }
                else
                {
                    Debug.LogError($"❌ Enemy type '{enemyLetter}' not found!");
                }
            }
            else
            {
                Debug.LogError($"❌ Invalid enemy group format: {trimmedGroup}");
            }
        }
    }

    // 🔄 Get enemy type by its identifier (e.g., 'A' for Archer)
    EnemyType GetEnemyTypeByIdentifier(char identifier)
    {
        foreach (EnemyType enemyType in enemyTypes)
        {
            if (enemyType.identifier[0] == identifier)
            {
                return enemyType;
            }
        }

        Debug.LogError($"❌ Enemy Type not found for identifier: {identifier}");
        return null;
    }

    private IEnumerator StartLevelWithDelay(int levelIndex)
{
    Debug.Log($"Starting level {levelIndex} with delay...");

    // Optional: add delay before starting level
    yield return new WaitForSeconds(0.5f);
    isGameStarted = true;
    // Start the actual level coroutine and wait for it to finish
    yield return StartCoroutine(HandleLevel(levelIndex));
}


    // ✅ ✅ NEW: Property to check when only the persistent 3 enemies are left
    private bool AreOnlyPersistentEnemiesLeft
    {
        get
        {
            List<EnemyManager> activeEnemies = EnemyManager.GetAllEnemies(); // Get all the currently active enemies
            return activeEnemies.Count <= 0; // Return true when only the 3 persistent enemies are left
        }
    }

    // 🔄 Spawns an enemy at a specific location based on spawn direction
    void SpawnEnemy(EnemyType enemyType, string spawnDirection)
    {
        if (enemyType == null)
        {
            Debug.LogError("❌ SpawnEnemy called with null EnemyType!");
            return;
        }

        enemySpawner.SpawnEnemyAtSpecificLocation(enemyType, spawnDirection);
    }

    public int GetCurrentLevelIndex()
    {
        return levelIndex;
    }

}
