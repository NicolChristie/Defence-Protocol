using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public float mapWidth = 10f;
    public float mapHeight = 6f;
    public float spawnBuffer = 1f;
    public GameObject player;
    public GameObject ship;
    public ShipHealthBar shipHealthBar;

    public void SpawnEnemyAtSpecificLocation(EnemyType enemyType, string spawnDirection)
    {
        Vector2 spawnPos = GetPositionBasedOnDirection(spawnDirection);
        GameObject newEnemy = Instantiate(enemyType.enemyPrefab, spawnPos, Quaternion.identity);

        EnemyManager enemyScript = newEnemy.GetComponent<EnemyManager>();
        if (enemyScript != null)
        {
            enemyScript.player = player;
            enemyScript.ship = ship;
            enemyScript.shipHealthBar = shipHealthBar;
            enemyScript.speed = enemyType.speed;
            enemyScript.maxHP = enemyType.maxHP;
            enemyScript.damage = enemyType.damage;
            enemyScript.coinAmount = enemyType.coinReward;
        }
        else
        {
            Debug.LogError("EnemyManager script not found on enemy prefab!");
        }
    }

    Vector2 GetPositionBasedOnDirection(string direction)
    {
        float x = 0f;
        float y = 0f;

        switch (direction)
        {
            case "L":
                x = -mapWidth - spawnBuffer;
                y = Random.Range(-mapHeight, mapHeight);
                break;
            case "R":
                x = mapWidth + spawnBuffer;
                y = Random.Range(-mapHeight, mapHeight);
                break;
            case "U":
                x = Random.Range(-mapWidth, mapWidth);
                y = mapHeight + spawnBuffer;
                break;
            case "D":
                x = Random.Range(-mapWidth, mapWidth);
                y = -mapHeight - spawnBuffer;
                break;
            default:
                Debug.LogError($"Invalid direction: {direction}");
                break;
        }

        return new Vector2(x, y);
    }
}
