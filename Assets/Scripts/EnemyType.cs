using UnityEngine;

[System.Serializable]
public class EnemyType
{
    public string name;         // Name of the enemy type
    public GameObject enemyPrefab; // The prefab to spawn
    public float speed;         // Enemy movement speed
    public int maxHP;           // Enemy max health
    public int damage;          // Damage dealt to player
    public int coinReward;      // Coins earned when this enemy is destroyed
    public string identifier;   // Character used to represent this enemy in level files
}
