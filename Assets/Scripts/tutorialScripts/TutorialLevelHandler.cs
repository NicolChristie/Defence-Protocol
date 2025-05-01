using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TutorialLevelHandler : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public GameObject player;
    public GameObject nextLevelButton;
    public EnemyType[] enemyTypes;
    private int levelIndex = 0;
    public TutorialManager tutorialManager;
    public TextAsset levelFile;

    private bool isLevelComplete = false;
    private bool isGameStarted = false;

    public void StartLevel()
    {
        isGameStarted = true;
        nextLevelButton.SetActive(false);
        StartCoroutine(HandleLevelWithDelay(1));
    }

    public IEnumerator HandleLevelWithDelay(int levelIndex)
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(HandleLevel(levelIndex));
    }

    public IEnumerator HandleLevel(int levelIndex)
    {
        if (!isGameStarted) yield break;

        Debug.Log($"🚀 Loading Level {levelIndex}");
        List<string> levelSetup = LoadLevelFromFile(levelFile, levelIndex);

        if (levelSetup.Count == 0)
        {
            Debug.LogError($"❌ Level {levelIndex} is empty!");
            yield break;
        }

        foreach (string wave in levelSetup)
        {
            SpawnEnemiesInWave(wave);
            yield return new WaitForSeconds(3f);
        }

        while (!AreOnlyPersistentEnemiesLeft)
        {
            yield return null;
        }

        if (WeaponNode.playerWeapon != null)
        {
            Debug.Log("Player is still carrying a weapon. Cannot finish level yet.");
            yield return StartCoroutine(WaitForWeaponToBePlacedDown());
            Debug.Log("Player has placed the weapon down. Proceeding with level completion.");
        }

        Debug.Log("🎉 Level Complete!");

        if (isLevelComplete) yield break;
        isLevelComplete = true;

        if (levelIndex < 8)
        {
            Debug.Log("showing tutorial manager and next level button");
            if (tutorialManager != null)
            {
                tutorialManager.TriggerNextStep(levelIndex);
            }
        }
        else
        {
            Debug.Log("showing shop and next level button");
            CoinManager.Instance.AddCoins(5);
            ShopManager.Instance.GenerateShop();
            nextLevelButton.SetActive(true);
            ShopManager.Instance.ShowShop();
        }

        Debug.Log($"Level {levelIndex} completed! Proceeding to next level.");
    }

    private IEnumerator WaitForWeaponToBePlacedDown()
    {
        while (WeaponNode.playerWeapon != null)
        {
            yield return null;
        }

        Debug.Log("Weapon has been placed down.");
    }

    public void ProceedToNextLevel()
    {
        levelIndex++;
        isLevelComplete = false;

        Debug.Log($"Next level button pressed! Current level: {levelIndex}");

        ShopManager.Instance.HideShop();
        List<string> nextLevelData = LoadLevelFromFile(levelFile, levelIndex);

        if (nextLevelData.Count > 0)
        {
            nextLevelButton.SetActive(false);
            StartCoroutine(StartLevelWithDelay(levelIndex));
        }
        else
        {
            Debug.Log("🎉 All levels complete!");
        }
    }

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

    void SpawnEnemiesInWave(string wave)
    {
        string cleanedWave = wave.Replace("[", "").Replace("]", "").Trim();
        string[] enemyGroups = cleanedWave.Split(',');

        foreach (string group in enemyGroups)
        {
            string trimmedGroup = group.Trim();
            if (string.IsNullOrEmpty(trimmedGroup)) continue;

            int count = 0;
            string enemyLetter = "";
            string spawnDirection = "";

            foreach (char c in trimmedGroup)
            {
                if (char.IsDigit(c))
                    count = count * 10 + (c - '0');
                else if (char.IsLetter(c))
                    enemyLetter += c;
            }

            if (trimmedGroup.Contains("."))
            {
                spawnDirection = trimmedGroup.Split('.')[1].ToUpper();
            }

            if (spawnDirection != "L" && spawnDirection != "R" && spawnDirection != "U" && spawnDirection != "D")
            {
                Debug.LogError($"❌ Invalid spawn direction: {spawnDirection} in wave {trimmedGroup}");
                continue;
            }

            if (count > 0 && enemyLetter.Length > 0)
            {
                EnemyType enemyType = GetEnemyTypeByIdentifier(enemyLetter[0]);

                if (enemyType != null)
                {
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
        yield return new WaitForSeconds(0.5f);
        isGameStarted = true;
        yield return StartCoroutine(HandleLevel(levelIndex));
    }

    private bool AreOnlyPersistentEnemiesLeft
    {
        get
        {
            List<EnemyManager> activeEnemies = EnemyManager.GetAllEnemies();
            return activeEnemies.Count <= 0;
        }
    }

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
