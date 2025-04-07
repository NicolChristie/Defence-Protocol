using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LevelHandler : MonoBehaviour
{
    public EnemySpawner enemySpawner; // Reference to EnemySpawner
    public GameObject player; // Reference to the player
    public GameObject nextLevelButton; // Reference to the Next Level Button
    public EnemyType[] enemyTypes; // Reference to different enemy types
    private int levelIndex = 0;
    private int spawnedEnemyCount = 0;

    public string levelFileName = "Levels.txt"; // Levels file name
    private bool isLevelComplete = false; // Flag to check if level is complete
    private bool isGameStarted = false; // Flag to track if the game has started

    private void Start()
    {
        // Show the next level button and shop at the start
        nextLevelButton.SetActive(true); // Show the next level button
        ShopManager.Instance.ShowShop(); // Show the shop at the start
    }

    // 🔄 Start the game when the user presses "Start"
    public void StartLevel()
    {
        isGameStarted = true;

        // Hide the next level button and the shop before starting the level
        nextLevelButton.SetActive(false);
        ShopManager.Instance.HideShop();

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
        List<string> levelSetup = LoadLevelFromFile(levelFileName, levelIndex);

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

        // Once all enemies are destroyed and the player isn't carrying a weapon, show the next level button and the shop
        Debug.Log("🎉 Level Complete!");

        // Add a check to ensure that the level hasn't already been completed
        if (isLevelComplete) yield break; // Prevent duplicate execution

        isLevelComplete = true;
        CoinManager.Instance.AddCoins(5); // Reward for finishing the level
        ShopManager.Instance.GenerateShop();
        nextLevelButton.SetActive(true); // Show the next level button
        ShopManager.Instance.ShowShop(); // Show the shop after completing the level
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

        // Generate a new shop before starting the next level
        ShopManager.Instance.GenerateShop();

        // Hide the shop when proceeding to the next level
        ShopManager.Instance.HideShop();

        List<string> nextLevelData = LoadLevelFromFile(levelFileName, levelIndex);

        if (nextLevelData.Count > 0)
        {
            Debug.Log($"🚀 Moving to Level {levelIndex}");
            nextLevelButton.SetActive(false); // Hide the next level button
            StartCoroutine(StartLevelWithDelay(levelIndex));

        }
        else
        {
            Debug.Log("🎉 All levels complete!");
        }
    }

    // 🔄 Load a specific level from the file
    public List<string> LoadLevelFromFile(string fileName, int targetLevel)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        List<string> levelData = new List<string>();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            bool isReading = false;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

                if (line.StartsWith($"Level {targetLevel}:"))
                {
                    isReading = true; // Start reading this level
                    Debug.Log($"Reading data for Level {targetLevel}...");
                    continue;
                }

                if (line.StartsWith("Level ") && isReading)
                {
                    Debug.Log($"Ending reading data for Level {targetLevel}");
                    break; // Stop reading when we reach the next level
                }

                if (isReading)
                {
                    levelData.Add(line.Trim()); // Add wave data
                }
            }   

            if (levelData.Count == 0)
            {
                Debug.LogError($"⚠️ No data found for Level {targetLevel}");
            }
        }
        else
        {
            Debug.LogError($"❌ Level file not found: {filePath}");
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
}
