using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelHandler : MonoBehaviour
{
    public EnemyType[] enemyTypes; // Different enemy types (e.g., Archers, Monsters, etc.)
    public EnemySpawner enemySpawner; // Reference to EnemySpawner
    public GameObject player; // Reference to the player
    public Weaponprefab[] weapons; // Array to store all weapon references (ensure weapons are referenced in the inspector)

    public string levelFileName = "Levels.txt"; // Levels file name
    private bool isLevelComplete = false; // Flag to check if level is complete
    private bool isGameStarted = false; // Flag to track if the game has started

    private void Start()
    {
        // Wait for the player to press the space bar to start the game
        StartCoroutine(WaitForSpaceBarToStartGame());
    }

    // 🔄 Wait for the player to press SPACE before starting the game
    private IEnumerator WaitForSpaceBarToStartGame()
    {
        Debug.Log("Press SPACE to start Level 1");

        // Wait until the player presses the space bar to start the first level
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        isGameStarted = true;
        StartCoroutine(HandleLevel(1)); // Start Level 1 after space bar press
    }

    // 🔄 Load only ONE level from the file (Stops reading when the next level starts)
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
                    continue;
                }

                if (line.StartsWith("Level ") && isReading)
                {
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

    // 🔄 Handle the level (spawns enemies)
    public IEnumerator HandleLevel(int levelIndex)
    {
        if (!isGameStarted)
        {
            yield break; // If the game hasn't started, do nothing
        }

        Debug.Log($"🚀 Loading Level {levelIndex}");

        List<string> levelSetup = LoadLevelFromFile(levelFileName, levelIndex);

        if (levelSetup.Count == 0)
        {
            Debug.LogError($"❌ Level {levelIndex} is empty!");
            yield break;
        }

        foreach (string wave in levelSetup)
        {
            if (wave.Contains("[") && wave.Contains("]"))
            {
                string[] enemiesInWave = wave.Trim('[', ']').Split(',');

                foreach (string enemy in enemiesInWave)
                {
                    SpawnEnemiesInWave(enemy);
                }
            }
            else
            {
                SpawnEnemiesInWave(wave);
            }

            yield return new WaitForSeconds(3f); // Delay between waves
        }

        yield return StartCoroutine(WaitForSpaceBar(levelIndex));
    }

    // 🔄 Wait for the player to press SPACE before starting the next level
    private IEnumerator WaitForSpaceBar(int currentLevelIndex)
    {
        isLevelComplete = true;
        Debug.Log($"✅ Level {currentLevelIndex} Complete! Press Space to continue.");

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null; // Wait until space is pressed
        }

        isLevelComplete = false; // Reset flag

        int nextLevelIndex = currentLevelIndex + 1; // Move to the next level

        if (LoadLevelFromFile(levelFileName, nextLevelIndex).Count > 0) // Check if next level exists
        {
            Debug.Log($"🚀 Moving to Level {nextLevelIndex}");
            StartCoroutine(HandleLevel(nextLevelIndex));
        }
        else
        {
            Debug.Log("🎉 All levels complete!");
        }
    }

    // 🔄 Spawns enemies based on wave instructions (e.g., "1M.L" means 1 Monster at Left)
    void SpawnEnemiesInWave(string wave)
    {
        string cleanedWave = wave.Replace("[", "").Replace("]", "").Trim(); // Remove brackets
        string[] enemyGroups = cleanedWave.Split(','); // Split into individual groups (e.g., ["1M.L", "3A.R"])

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
