using UnityEngine;

[System.Serializable] // Allows editing in the Unity Inspector
public class EnemyType
{
    public string name;       // Example: "Archer"
    public string identifier; // Example: "A"
    public GameObject enemyPrefab; // The actual prefab to spawn
    public float speed;
    public int maxHP;
    public int damage;
}
