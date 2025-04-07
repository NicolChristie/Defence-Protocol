using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Default prefab, will be overridden by specific enemy types
    public float spawnInterval = 3f;
    public float mapWidth = 10f;
    public float mapHeight = 6f;
    public float spawnBuffer = 1f; // Offset so enemies spawn slightly off-screen
    public GameObject player; // Reference to the player
    public GameObject ship; // Reference to the ship GameObject
    public ShipHealthBar shipHealthBar; // Reference to the ship's health bar

    // 🔄 Spawns an enemy at a specific location based on spawn direction
    public void SpawnEnemyAtSpecificLocation(EnemyType enemyType, string spawnDirection)
    {
        Vector2 spawnPos = GetPositionBasedOnDirection(spawnDirection);
        GameObject newEnemy = Instantiate(enemyType.enemyPrefab, spawnPos, Quaternion.identity);

        // Assign the references dynamically after the enemy is instantiated
        EnemyManager enemyScript = newEnemy.GetComponent<EnemyManager>();
        if (enemyScript != null)
        {
            // Assign the necessary references to the enemy
            enemyScript.player = player;
            enemyScript.ship = ship;
            enemyScript.shipHealthBar = shipHealthBar; // Assign the ship's health bar

            // Also assign specific enemy properties
            enemyScript.speed = enemyType.speed;
            enemyScript.maxHP = enemyType.maxHP;
            enemyScript.damage = enemyType.damage;
            enemyScript.coinAmount = enemyType.coinReward; // NEW: Assign coin reward
        }
        else
        {
            Debug.LogError("EnemyManager script not found on enemy prefab!");
        }
    }

    // 🔄 Get position based on spawn direction
    Vector2 GetPositionBasedOnDirection(string direction)
    {
        float x = 0f;
        float y = 0f;

        switch (direction)
        {
            case "L":
                x = -mapWidth - spawnBuffer; // Spawn on the left
                y = Random.Range(-mapHeight, mapHeight);
                break;
            case "R":
                x = mapWidth + spawnBuffer; // Spawn on the right
                y = Random.Range(-mapHeight, mapHeight);
                break;
            case "U":
                x = Random.Range(-mapWidth, mapWidth);
                y = mapHeight + spawnBuffer; // Spawn at the top
                break;
            case "D":
                x = Random.Range(-mapWidth, mapWidth);
                y = -mapHeight - spawnBuffer; // Spawn at the bottom
                break;
            default:
                Debug.LogError($"❌ Invalid direction: {direction}");
                break;
        }

        return new Vector2(x, y);
    }
}
