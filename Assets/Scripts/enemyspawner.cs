using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public GameObject leftCollider;
    public GameObject rightCollider;
    public GameObject topCollider;
    public GameObject bottomCollider;
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
                x = leftCollider.transform.position.x;
                y = Random.Range(bottomCollider.transform.position.y, topCollider.transform.position.y);
                break;
            case "R":
                x = rightCollider.transform.position.x;
                y = Random.Range(bottomCollider.transform.position.y, topCollider.transform.position.y);
                break;
            case "U":
                x = Random.Range(leftCollider.transform.position.x, rightCollider.transform.position.x);
                y = topCollider.transform.position.y;
                break;
            case "D":
                x = Random.Range(leftCollider.transform.position.x, rightCollider.transform.position.x);
                y = bottomCollider.transform.position.y;
                break;
            default:
                Debug.LogError($"Invalid direction: {direction}");
                break;
        }

        return new Vector2(x, y);
    }
}
